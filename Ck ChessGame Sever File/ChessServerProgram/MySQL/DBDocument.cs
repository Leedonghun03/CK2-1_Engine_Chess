using MySql.Data.MySqlClient;

namespace ChessServerProgram.MySQL
{
    public class DBDocument<T> where T : class
    {
        public int Id { get; set; } = -1;

        public bool Save(MySqlConnection conn)
        {
            return DataModel<T>.Get().Save(this, conn);
        }

        public bool Delete(MySqlConnection conn)
        {
            return DataModel<T>.Get().Delete(this, conn);
        }

        public static T? FindById(int id, MySqlConnection conn)
        {
            return DataModel<T>.Get().FindById(id, conn);
        }
    }
}
