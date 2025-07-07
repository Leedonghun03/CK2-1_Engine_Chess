using ChessServerProgram.MySQL;
using EndoAshu.Chess.Server.User;
using EndoAshu.Chess.User;
using MySql.Data.MySqlClient;
using Runetide.Util;
using SharpCompress.Common;
using System.Text.Json;

namespace ChessServerProgram
{
    public class ServerUserAccountService : IUserAccountService
    {
        [DBTableName("user_account")]
        private class UserAccountDataDocument : DBDocument<UserAccountDataDocument>
        {
            [DBColumnName("uuid")]
            [DBColumnUnique]
            public UUID UniqueId { get; set; } = UUID.NULL;

            [DBColumnName("username")]
            [DBColumnUnique]
            public string Username { get; set; } = string.Empty;

            [DBColumnName("user_id")]
            [DBColumnUnique]
            public string UserId { get; set; } = string.Empty;

            [DBColumnName("password")]
            public string Password { get; set; } = string.Empty;

            [DBColumnName("email")]
            public string Email { get; set; } = string.Empty;

            [DBColumnName("win")]
            public int Win { get; set; } = 0;

            [DBColumnName("draw")]
            public int Draw { get; set; } = 0;

            [DBColumnName("lose")]
            public int Lose { get; set; } = 0;

            public UserAccountDataDocument()
            {

            }

            public UserAccountDataDocument(UserAccount.Data data)
            {
                UniqueId = data.UniqueId;
                Username = data.Username;
                UserId = data.Id;
                Password = data.Password;
                Win = data.Win;
                Draw = data.Draw;
                Lose = data.Lose;
            }

            public static UserAccountDataDocument? FindByUUID(UUID uuid, MySqlConnection conn)
            {
                var list = DataModel<UserAccountDataDocument>.Get().FindBy("uuid", uuid, conn);
                if (list.Count > 0)
                    return list[0];
                return null;
            }

            public static UserAccountDataDocument? FindByUserId(string userid, MySqlConnection conn)
            {
                var list = DataModel<UserAccountDataDocument>.Get().FindBy("user_id", userid, conn);
                if (list.Count > 0)
                    return list[0];
                return null;
            }

            public void CopyFrom(UserAccount.Data data)
            {
                UniqueId = data.UniqueId;
                Username = data.Username;
                UserId = data.Id;
                Password = data.Password;
                Win = data.Win;
                Draw = data.Draw;
                Lose = data.Lose;
            }
        }

        internal class ServerUserAccountImpl : ServerUserAccount
        {
            private ServerUserAccountService Service { get; }

            public ServerUserAccountImpl(ServerUserAccountService service, Data data)
                : base(data)
            {
                Service = service;
            }

            public override void Save(bool cacheOnly = false)
            {
                base.Save(cacheOnly);

                if (!cacheOnly)
                {
                    var conn = Service.MySQL.GetConnection();
                    var i = UserAccountDataDocument.FindByUUID(this.data.UniqueId, conn);
                    if (i != null)
                    {
                        i.CopyFrom(this.data);
                        i.Save(conn);
                    } else
                    {
                        new UserAccountDataDocument(this.data).Save(conn);
                    }
                }

                Service.Redis.Fetch(redisInstance =>
                {
                    redisInstance.Set($"user_account:u2d:{UniqueId}", JsonSerializer.Serialize(data));
                    redisInstance.Set($"user_account:u4d:{Id}", JsonSerializer.Serialize(data));
                });
            }
        }
        private RedisClient Redis { get; }
        private MySQLClient MySQL { get; }

        public ServerUserAccountService(MySQLClient mysql, RedisClient redis)
        {
            MySQL = mysql;
            Redis = redis;
        }

        public ServerUserAccount? Get(string id)
        {
            ServerUserAccount? account = null;
            Redis.Fetch(redisInstance =>
            {
                try
                {
                    if (redisInstance.ContainsKey($"user_account:u4d:{id}"))
                    {
                        string? res = redisInstance.Get($"user_account:u4d:{id}");
                        if (res != null)
                        {
                            account = new ServerUserAccountImpl(this, JsonSerializer.Deserialize<UserAccount.Data>(res)!);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error fetching user account from Redis: {e.Message}");
                }
            });

            return account;
        }

        public ServerUserAccount? Get(UUID uuid)
        {
            ServerUserAccount? account = null;
            Redis.Fetch(redisInstance =>
            {
                try
                {
                    if (redisInstance.ContainsKey($"user_account:u2d:{uuid}"))
                    {
                        string? res = redisInstance.Get($"user_account:u2d:{uuid}");
                        if (res != null)
                        {
                            account = new ServerUserAccountImpl(this, JsonSerializer.Deserialize<UserAccount.Data>(res)!);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error fetching user account from Redis: {e.Message}");
                }
            });

            return account;
        }

        public ServerUserAccount? Login(string id, string password)
        {
            var doc = UserAccountDataDocument.FindByUserId(id, MySQL.GetConnection());
            if (doc == null)
                return null;

            if (doc.Password != password)
                return null;

            var res = new ServerUserAccountImpl(this, new UserAccount.Data
            {
                UniqueId = doc.UniqueId,
                Username = doc.Username,
                Id = doc.UserId,
                Password = doc.Password,
                Win = doc.Win,
                Draw = doc.Draw,
                Lose = doc.Lose
            });
            res.Save(true); // Save to cache only
            return res;
        }
    }
}
