using EndoAshu.Chess.Server.User;
using Runetide.Service;
using System.Collections.Generic;

namespace EndoAshu.Chess.Server
{
    public class ServerServices
    {
        static ServerServices()
        {
            RegisterService(1000, new UserAccountServiceMockup());
        }

        public static TService GetService<TService>() where TService : IService => SimpleServiceManager.GetManager().GetService<TService>();
        public static IEnumerable<TService> GetServices<TService>() where TService : IService => SimpleServiceManager.GetManager().GetServices<TService>();
        public static void RegisterService<TService>(int priority, TService service) where TService : IService => SimpleServiceManager.GetManager().RegisterService(priority, service);
    }
}
