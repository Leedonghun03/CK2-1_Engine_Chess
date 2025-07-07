namespace ChessServerProgram.MySQL
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DBTableNameAttribute : System.Attribute
    {
        public string Name { get; }

        public DBTableNameAttribute(string name)
        {
            Name = name;
        }
    }
}