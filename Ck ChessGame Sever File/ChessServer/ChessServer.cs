using EndoAshu.Chess.Server.Room;
using EndoAshu.Chess.Server.User;
using Runetide.Attr;
using Runetide.Net;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;
using Runetide.Util.Logging;
using System.Collections.Concurrent;

namespace EndoAshu.Chess.Server
{
    public class ChessServer
    {
        public static readonly AttributeKey<ChessServer> CHESS_SERVER = AttributeKey<ChessServer>.Get("chess_server");

        private readonly ConcurrentDictionary<UUID, UserCache> userCache = new ConcurrentDictionary<UUID, UserCache>();

        private readonly RawServerSocket runner;
        internal RawServerSocket Socket => runner;
        
        public int ClientCount => runner.Count;
        public RoomManager Rooms { get; }
        public double HeartbeatInterval
        {
            get => runner.HeartbeatInterval;
            set => runner.HeartbeatInterval = value;
        }

        public ILogger? Logger
        {
            get => runner.Logger;
        }

        public ChessServer(int port, int maxConnections = 100)
        {
            runner = new RawServerSocket(System.Net.Sockets.ProtocolType.Tcp, maxConnections, port);
            IPacketDescriptor descriptor = new PacketDescriptorImpl();
            NetworkRegistry.Apply(descriptor);
            runner.PacketDescriptor.Add(descriptor);
            Rooms = new RoomManager(this);
        }

        public void Start()
        {
            runner.Connected += Socket_Connected;
            runner.Start();
        }

        public void Stop()
        {
            runner.Stop();
            runner.Connected -= Socket_Connected;
        }

        private void Socket_Connected(object obj, NetworkContext e)
        {
            e.GetAttribute(CHESS_SERVER).Set(this);
        }

        public void OnTick()
        {
            runner.OnTick();
            Rooms.OnTick();
        }

        internal UserCache GetOrCreateUserCache(ServerUserAccount account, NetworkContext ctx)
        {
            UserCache cache = userCache.GetOrAdd(account.UniqueId, new UserCache(null));
            cache.Ctx = ctx;
            return cache;
        }
    }
}
