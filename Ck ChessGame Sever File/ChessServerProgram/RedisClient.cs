using Runetide.Util.Functions;
using StackExchange.Redis;

namespace ChessServerProgram
{
    public class RedisClient : IDisposable
    {
        public class Instance : IDisposable
        {
            private IDatabase? db;

            protected internal Instance(RedisClient client)
            {
                db = client.multiplex!.GetDatabase();
            }

            public bool ContainsKey(string key)
            {
                if (db == null) throw new InvalidOperationException("Database connection is not initialized.");
                return db.KeyExists(key);
            }

            public void Set(string key, string value)
            {
                if (db == null) throw new InvalidOperationException("Database connection is not initialized.");
                db.StringSet(key, value, expiry: TimeSpan.FromMinutes(5));
            }

            public string? Get(string key)
            {
                if (db == null) throw new InvalidOperationException("Database connection is not initialized.");
                var v = db.StringGet(key);
                if (v.IsNull)
                    return null;
                return v.ToString();
            }

            public void Dispose()
            {
                db = null;
            }
        }

        private ConnectionMultiplexer? multiplex;

        public RedisClient(string connString)
        {
            multiplex = ConnectionMultiplexer.Connect(connString);
        }

        public void Fetch(Consumer<Instance> consumer)
        {
            using(var redisInstance = new Instance(this))
            {
                consumer.Invoke(redisInstance);
            }
        }

        public void Dispose()
        {
            multiplex?.Dispose();
            multiplex = null;
        }
    }
}
