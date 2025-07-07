//Entry Point

//#define USE_DOCKER

using ChessServerProgram;
using EndoAshu.Chess.Server;
using EndoAshu.Chess.Server.User;
using EndoAshu.Chess.User;
using Runetide.Util;
using Runetide.Util.Logging;

const int SERVER_PORT = 1557;

#region test account

void InitTestAccount(ServerUserAccountService service)
{
    List<UserAccount.Data> list = new List<UserAccount.Data>();

    for (int i = 0; i < 20; ++i)
    {
        string suffix = string.Format("{0:D2}", i + 1);
        list.Add(new UserAccount.Data()
        {
            UniqueId = UUID.Create(),
            Id = $"cktest{suffix}",
            Username = $"CKTest_{suffix}",
            Password = "1234",
            Win = 0,
            Lose = 0,
            Draw = 0
        });
    }

    foreach (var account in list)
    {
        if (service.Get(account.Id) == null)
        {
            var impl = new ServerUserAccountService.ServerUserAccountImpl(service, account);
            impl.Save(false);
        }
    }
}

#endregion


#if USE_DOCKER
using (MySQLClient MYSQL = new MySQLClient("mysql", "root", "1q2w3e4r!"))
using (RedisClient REDIS = new RedisClient("redis:6379"))
{

    var suas = new ServerUserAccountService(MYSQL, REDIS);
    InitTestAccount(suas);
    ServerServices.RegisterService<IUserAccountService>(1, suas);
#endif
    ChessServer server = new ChessServer(SERVER_PORT);
    server.Start();

    ILogger Logger = server.Logger!;
    Logger.OnLogging += (item) =>
    {
        Console.WriteLine(item.ToString());
    };
    Logger.Info($"Server Listening on 0.0.0.0:{SERVER_PORT}");
    Logger.MinLevel = LogLevel.TRACE;

    bool killSwitch = false;


    int lastCC = 0;

    while (!killSwitch)
    {
        server.OnTick();
        Thread.Sleep(1000);
        int curCC = server.ClientCount;
        if (curCC != lastCC)
        {
            lastCC = curCC;
            Logger.Info($"{curCC} clients.");
        }
    }

    server.Stop();


#if USE_DOCKER 
}
#endif