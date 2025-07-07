using Runetide.Service;
using Runetide.Util;

namespace EndoAshu.Chess.Server.User
{
    public interface IUserAccountService : IService
    {
        ServerUserAccount? Login(string id, string password);
        ServerUserAccount? Get(UUID uuid);
    }
}
