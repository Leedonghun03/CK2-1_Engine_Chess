using EndoAshu.Chess.Client;
using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.Room;
using EndoAshu.Chess.Server;
using Runetide.Util;
using System.Net;
void assert(bool flag)
{
    if (!flag)
        throw new Exception();
}

ChessServer server = new ChessServer(1557);
server.Start();

ChessClient[] clients = {
    new ChessClient(new IPEndPoint(IPAddress.Loopback, 1557)),
    new ChessClient(new IPEndPoint(IPAddress.Loopback, 1557)),
    new ChessClient(new IPEndPoint(IPAddress.Loopback, 1557))
};

int ticks = 0;
bool ks = false;

var thread = new Thread(new ThreadStart(() =>
{
    while (!ks)
    {
        server.OnTick();
        foreach (var client in clients)
            client.OnTick();

        if (++ticks >= 20)
        {
            ticks = 0;
            Console.WriteLine($"Connected {server.ClientCount} User.");
        }
        Thread.Sleep(100);
    }
}));

int idx = 0;
foreach (var client in clients)
{
    client.Start();
    await (client.State as GameLoginState)!.Login($"user_{++idx}", "1q2w3e4r!");
}
thread.Start();

(clients[0].State as GameLobbyState)!.ChatReceived += GameLobbyState_ChatReceived;
(clients[1].State as GameLobbyState)!.ChatReceived += GameLobbyState_ChatReceived;
(clients[2].State as GameLobbyState)!.ChatReceived += GameLobbyState_ChatReceived;

void GameLobbyState_ChatReceived(GameLobbyState.ChatItem t)
{
    Console.WriteLine(t);
}

(clients[0].State as GameLobbyState)!.SendChat("sadf");

//ks = true;
thread.Join();
Console.WriteLine("END");