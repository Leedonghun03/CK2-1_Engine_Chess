using EndoAshu.Chess.Client.Room;
using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.Client.User;
using EndoAshu.Chess.Room;
using EndoAshu.Chess.User;
using Runetide.Attr;
using Runetide.Net;
using Runetide.Packet;
using Runetide.Util.Functions;
using Runetide.Util.Logging;
using System;
using System.Net;
using System.Net.Sockets;

namespace EndoAshu.Chess.Client
{
    public class ChessClient : IDisposable
    {
        public static readonly AttributeKey<ChessClient> CHESS_CLIENT = AttributeKey<ChessClient>.Get("chess_client");

        private readonly RawClientSocket runner;

        public ClientUserAccount? Account => runner.Context.GetAttribute(UserAccount.ACCOUNT_KEY).Get() as ClientUserAccount;

        public ClientRoom? CurrentRoom { get; internal set; }

        public event EventHandler<RoomQuitPacket.QuitStatus>? OnQuitRoom;

        public bool IsConnected => runner.Context.IsConnected;

        public GameState State { get; private set; }

        public ILogger? Logger
        {
            get => runner.Logger;
        }

        public double HeartbeatInterval
        {
            get => runner.HeartbeatInterval;
            set => runner.HeartbeatInterval = value;
        }

        private readonly IPEndPoint remotePoint;

        public ChessClient(IPEndPoint remotePoint)
        {
            this.remotePoint = remotePoint;
            State = new GameLoginState(this);
            runner = new RawClientSocket(ProtocolType.Tcp, 100);
            PacketDescriptorImpl descriptor = new PacketDescriptorImpl();
            NetworkRegistry.Apply(descriptor);
            runner.PacketDescriptor.Add(descriptor);
        }

        internal void UpdateState(GameState state)
        {
            State?.Dispose();
            State = state;
        }

        public void Start() => runner.Connect(remotePoint);
        public void Stop() => runner.Close(runner.Context);
        public void OnTick() => runner.OnTick();

        public void RefreshState()
        {
            if (Account == null)
            {
                UpdateState(new GameLoginState(this));
            }
            else if (CurrentRoom == null)
            {
                UpdateState(new GameLobbyState(this));
            }
            else if (!CurrentRoom.PlayingData.IsPlaying)
            {
                UpdateState(new GameRoomState(this));
            }
            else
            {
                UpdateState(new GameInState(this));
            }
        }

        public void Send(IPacket pk)
        {
            runner.Context.GetAttribute(CHESS_CLIENT).Set(this);
            runner.Context.Send(pk);
        }

        public void Dispose()
        {
            OnQuitRoom = null;
            runner?.Dispose();
        }

        public void AddOnReceivePacketListener(Consumer<IPacket> listener)
        {
            runner.OnReceived += listener;
        }

        public void RemoveOnReceivePacketListener(Consumer<IPacket> listener)
        {
            runner.OnReceived -= listener;
        }

        public void RunOnMainThread(Runnable action)
        {
            runner.EnqueueAction(action);
        }
    }
}
