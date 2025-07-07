using MySql.Data.MySqlClient;
using Runetide.Util;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;

namespace ChessServerProgram.MySQL
{
    public class DataModel<T> where T : class
    {
        private class Column
        {
            public string Name { get; }
            public PropertyInfo Property { get; }
            public string DataType { get; }
            public bool IsUnique { get; }
            public bool IsNullable { get; }

            public Column(string name, PropertyInfo info)
            {
                this.Name = name;
                this.Property = info;
                this.DataType = ParseDataType();
                this.IsUnique = info.GetCustomAttribute<DBColumnUniqueAttribute>() != null;
                this.IsNullable = info.GetCustomAttribute<DBColumnNullableAttribute>() != null;
            }

            public void Apply(object dataObject, MySqlDataReader reader)
            {
                if (Property.PropertyType == typeof(UUID))
                {
                    string s = (reader.GetValue(Name) as string)!;
                    Property.SetValue(dataObject, UUID.Parse(s));
                } else
                    Property.SetValue(dataObject, reader.GetValue(Name));
            }

            public void Apply(object dataObject, MySqlCommand command, string maskingName)
            {
                if (Property.PropertyType == typeof(UUID))
                {
                    UUID uuid = (UUID)Property.GetValue(dataObject)!;
                    command.Parameters.AddWithValue(maskingName, uuid.ToString());
                }
                else
                    command.Parameters.AddWithValue(maskingName, Property.GetValue(dataObject));
            }

            private string ParseDataType()
            {
                Type t = Property.PropertyType;
                if (t == typeof(byte)) return "tinyint";
                if (t == typeof(short)) return "smallint";
                if (t == typeof(int)) return "int";
                if (t == typeof(long)) return "bigint";
                if (t == typeof(decimal)) return "decimal(30, 5)";
                if (t == typeof(float)) return "float";
                if (t == typeof(double)) return "double";
                if (t == typeof(UUID)) return "varchar(40)";
                DBColumnStringAttribute? str = Property.GetCustomAttribute<DBColumnStringAttribute>();
                if (str != null && str.Length > 10)
                    return $"varchar({str.Length})";
                return "varchar(40)";
            }
        }

        private readonly Dictionary<string, Column> props = new Dictionary<string, Column>();
        public string TableName { get; }
        public string DBName { get; }

        private readonly ConstructorInfo newInstance;

        private static readonly ConcurrentDictionary<Type, object> models = new ConcurrentDictionary<Type, object>();

        private bool init = false;

        public static DataModel<T> Get()
        {
            return (DataModel<T>) models.GetOrAdd(typeof(T), new DataModel<T>("main"));
        }

        private DataModel(string dbName)
        {
            Type t = typeof(T);
            newInstance = t.GetConstructor(Type.EmptyTypes)!;
            DBName = dbName;
            DBTableNameAttribute? tableAttr = t.GetCustomAttribute<DBTableNameAttribute>();
            TableName = tableAttr != null ? tableAttr.Name : t.Name.ToLower();
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var prop in properties)
            {
                if (prop.Name == "Id") continue;
                DBColumnNameAttribute? nameAttr = prop.GetCustomAttribute<DBColumnNameAttribute>();
                string name = nameAttr != null ? nameAttr.Name : prop.Name;
                props[name] = new Column(name, prop);
            }
        }

        public bool Save(DBDocument<T> doc, MySqlConnection conn)
        {
            Init(conn);
            if (doc.Id >= 0)
            {
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var prop in props.Values)
                        {
                            using (MySqlCommand cmd = new MySqlCommand($"UPDATE {DBName}.{TableName} SET {prop.Name}=?Value WHERE idx=?Id", conn))
                            {
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("?Id", doc.Id);
                                prop.Apply(doc, cmd, "?Value");
                                cmd.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                        return true;
                    } catch(Exception e)
                    {
                        Console.WriteLine(e);
                        trans.Rollback();
                        return false;
                    }
                }
            } else
            {
                string cmdQ = "";
                string valQ = "";
                int idx = 0;
                foreach(var prop in props.Values)
                {
                    if (idx > 0)
                    {
                        cmdQ += ", ";
                        valQ += ", ";
                    }
                    cmdQ += prop.Name;
                    valQ += $"?Val{idx}";
                    ++idx;
                }

                try
                {
                    using (MySqlCommand cmd = new MySqlCommand($"INSERT INTO {DBName}.{TableName} ({cmdQ}) VALUES ({valQ})", conn))
                    {
                        int i = 0;
                        foreach (var prop in props.Values)
                        {
                            prop.Apply(doc, cmd, $"?Val{i++}");
                        }
                        return cmd.ExecuteNonQuery() > 0;
                    }
                } catch
                {
                    return false;
                }
            }
        }

        public List<T> FindBy(string column, object target, MySqlConnection conn)
        {
            Init(conn);
            List<T> arr = new List<T>();
            using (MySqlCommand cmd = new MySqlCommand($"SELECT * FROM {DBName}.{TableName} WHERE {column}=?Val", conn))
            {
                cmd.Parameters.AddWithValue("?Val", target);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        object t = newInstance.Invoke(new object[] { });
                        foreach (var prop in props.Values)
                        {
                            prop.Apply(t, reader);
                        }
                        T? res = (T?)t;
                        if (res != null && res is DBDocument<T> doc)
                        {
                            doc.Id = reader.GetInt32("idx");
                            arr.Add(res);
                        }
                    }
                }
            }
            return arr;
        }

        public T? FindById(int id, MySqlConnection conn)
        {
            var list = FindBy("idx", id, conn);
            if (list.Count > 0)
                return list[0];
            return null;
        }

        public bool Delete(DBDocument<T> doc, MySqlConnection conn)
        {
            using (MySqlCommand cmd = new MySqlCommand($"DELETE FROM {DBName}.{TableName} WHERE idx=?Id", conn))
            {
                cmd.Parameters.AddWithValue("?Id", doc.Id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public void Init(MySqlConnection conn)
        {
            if (init) return;
            init = true;
            using(MySqlCommand cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {DBName}", conn))
            {
                cmd.ExecuteNonQuery();
            }
            using (MySqlCommand cmd = new MySqlCommand($"CREATE TABLE IF NOT EXISTS {DBName}.{TableName} (idx int AUTO_INCREMENT PRIMARY KEY)", conn))
            {
                cmd.ExecuteNonQuery();
            }

            foreach(var prop in props.Values)
            {
                string MoreAttr = $" {(prop.IsNullable && !prop.IsUnique ? "NULL" : "NOT NULL")}";
                if (prop.IsUnique) MoreAttr = $"{MoreAttr} UNIQUE";

                using(MySqlCommand ck = new MySqlCommand($"SELECT COUNT(*) FROM information_schema.columns WHERE table_schema='{DBName}' AND table_name='{TableName}' AND column_name='{prop.Name}'", conn))
                {
                    int c = Convert.ToInt32(ck.ExecuteScalar());
                    if (c == 0)
                    {
                        using (MySqlCommand cmd = new MySqlCommand($"ALTER TABLE {DBName}.{TableName} ADD COLUMN {prop.Name} {prop.DataType}{MoreAttr}", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

#if DEBUG
            using (MySqlCommand cmd = new MySqlCommand($"DESC {DBName}.{TableName}", conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string field = reader["Field"].ToString();
                        string type = reader["Type"].ToString();
                        string isNullable = reader["Null"].ToString();
                        string key = reader["Key"].ToString();
                        string defaultValue = reader["Default"]?.ToString() ?? "NULL";
                        string extra = reader["Extra"].ToString();
                        Console.WriteLine($"{field} | {type} | {isNullable} | {key} | {defaultValue} | {extra}");
                    }
                }
            }
#endif
        }
    }
}
