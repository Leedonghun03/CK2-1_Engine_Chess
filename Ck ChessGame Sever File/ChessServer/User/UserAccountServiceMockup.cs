using Runetide.Util;
using System.Collections.Concurrent;

namespace EndoAshu.Chess.Server.User
{
    /// <summary>
    /// In-Memory 서비스
    /// Password는 평문으로 저장
    /// </summary>
    internal class UserAccountServiceMockup : IUserAccountService
    {
        private readonly ConcurrentDictionary<UUID, ServerUserAccount> cache = new ConcurrentDictionary<UUID, ServerUserAccount>();

        public ServerUserAccount? Login(string id, string password)
        {
            foreach(var i in cache.Values)
            {
                if (i.Id == id)
                {
                    if (i.Password == password)
                    {
                        // 로그인 성공
                        return i;
                    }
                    return null;
                }
            }

            ServerUserAccount.Data data = new ServerUserAccount.Data();
            data.Id = id;
            data.Username = id;
            data.Password = password;
            data.UniqueId = UUID.Create();
            var account = new ServerUserAccount(data);
            cache.TryAdd(data.UniqueId, account);
            return account;
        }
        public ServerUserAccount? Get(UUID uuid)
        {
            return cache.TryGetValue(uuid, out ServerUserAccount res) ? res : null;
        }
    }
}
