using EndoAshu.Chess.Client.Lobby;
using EndoAshu.Chess.Client.Room;
using EndoAshu.Chess.Room;
using Runetide.Util;
using Runetide.Util.Functions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EndoAshu.Chess.Client.State
{
    public class GameLobbyState : GameState
    {
        public class ChatItem
        {
            public UUID UniqueId { get; }
            public string Name { get; }
            public string Message { get; }

            public ChatItem(UUID uniqueId, string name, string message)
            {
                UniqueId = uniqueId;
                Name = name;
                Message = message;
            }

            public override string ToString()
            {
                return $"<{Name}:{UniqueId}> {Message}";
            }
        }

        public event Consumer<ChatItem>? ChatReceived;

        public  GameLobbyState(ChessClient client) : base(client)
        {

        }

        internal void OnReceiveChat(UUID uniqueId, string name, string message)
        {
            ChatItem chatItem = new ChatItem(uniqueId, name, message);
            ChatReceived?.Invoke(chatItem);
        }

        #region SendChat
        public void SendChat(string message)
        {
            ClientSideLobbyChatPacket pk = new ClientSideLobbyChatPacket(message);
            Client.Send(pk);
        }
        #endregion

        #region RoomJoin
        internal TaskCompletionSource<JoinRoomStatus>? JoinResponse;
        public async Task<JoinRoomStatus> JoinRoom(UUID roomId, int timeoutMS = 1000)
        {
            return await JoinRoom(roomId, string.Empty, timeoutMS);
        }

        public async Task<JoinRoomStatus> JoinRoom(UUID roomId, string password, int timeoutMS = 1000)
        {
            if (JoinResponse != null)
                return JoinRoomStatus.FAILED;

            JoinResponse = new TaskCompletionSource<JoinRoomStatus>();
            Client.Send(new ClientSideRoomJoinPacket.Request(roomId, password));

            JoinRoomStatus result;
            try
            {
                result = await JoinResponse.Task.WaitAsync(timeoutMS);
            }
            catch
            {
                result = JoinRoomStatus.TIMEOUT;
            }

            JoinResponse = null;

            if (Client.CurrentRoom != null && result == JoinRoomStatus.TIMEOUT)
                return JoinRoomStatus.ALREADY_INSIDE_ROOM;
            return result;
        }

        internal void InternalOnListRoom(JoinRoomStatus code)
        {
            JoinResponse?.TrySetResult(code);
        }
        #endregion

        #region RoomCreate
        internal TaskCompletionSource<Tuple<RoomCreatePacket.CreateStatus, UUID>>? RoomCreateResponse;
        public async Task<Tuple<RoomCreatePacket.CreateStatus, UUID>> CreateRoom(RoomOptions options, int timeoutMS = 1000)
        {
            if (RoomCreateResponse != null)
                return new Tuple<RoomCreatePacket.CreateStatus, UUID>(RoomCreatePacket.CreateStatus.FAILED, UUID.NULL);

            RoomCreateResponse = new TaskCompletionSource<Tuple<RoomCreatePacket.CreateStatus, UUID>>();
            Client.Send(new ClientSideRoomCreatePacket.Request(options));

            Tuple<RoomCreatePacket.CreateStatus, UUID> result;
            try
            {
                result = await RoomCreateResponse.Task.WaitAsync(timeoutMS);
            }
            catch
            {
                result = new Tuple<RoomCreatePacket.CreateStatus, UUID>(RoomCreatePacket.CreateStatus.TIMEOUT, UUID.NULL);
            }

            RoomCreateResponse = null;
            return result;
        }

        public void IUnderstandThisMethodIsNotRecommendButWantResetRoomCreateResponse()
        {
            if (RoomCreateResponse != null)
            {
                RoomCreateResponse.TrySetCanceled();
            }
            RoomCreateResponse = null;
        }

        internal void InternalOnCreateRoom(RoomCreatePacket.CreateStatus status, UUID? roomId)
        {
            Client.Logger?.Debug($"Receive Room Create - {status} {roomId}");
            RoomCreateResponse?.TrySetResult(new Tuple<RoomCreatePacket.CreateStatus, UUID>(status, roomId ?? UUID.NULL));
        }
        #endregion

        #region RoomList
        internal TaskCompletionSource<RoomListPacket.Data>? RoomListResponse;
        public async Task<RoomListPacket.Data> RoomList(int page, int itemsPerPage = 10, int timeoutMS = 1000)
        {
            if (RoomListResponse != null)
                return new RoomListPacket.Data(page, RoomListPacket.DataStatus.FAILED);

            RoomListResponse = new TaskCompletionSource<RoomListPacket.Data>();
            Client.Send(new ClientSideRoomListPacket.Request(page, itemsPerPage));

            RoomListPacket.Data result;
            try
            {
                result = await RoomListResponse.Task.WaitAsync(timeoutMS);
            }
            catch
            {
                result = new RoomListPacket.Data(page, new List<RoomListPacket.Item>().AsReadOnly(), 1);
            }
            RoomListResponse = null;
            return result;
        }

        internal void InternalOnRoomList(RoomListPacket.Data data)
        {
            RoomListResponse?.TrySetResult(data);
        }
        #endregion

        public override void Dispose()
        {
            ChatReceived = null;
            JoinResponse?.SetCanceled();
            RoomCreateResponse?.SetCanceled();
            RoomListResponse?.SetCanceled();
        }
    }
}
