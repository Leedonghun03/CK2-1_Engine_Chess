using MySql.Data.MySqlClient;

namespace ChessServerProgram
{
    public class MySQLClient : IDisposable
    {
        private MySqlConnection? Connection;

        public MySQLClient(string host, string user, string password)
        {
            Connection = new MySqlConnection($"SERVER={host};UID={user};PWD={password};CHARSET=utf8");
            Connection.Open();
        }

        public MySqlConnection GetConnection()
        {
            return Connection ?? throw new InvalidOperationException("Connection is not initialized.");
        }

        public void Dispose()
        {
            Connection?.Dispose();
            Connection = null;
        }
    }
}
