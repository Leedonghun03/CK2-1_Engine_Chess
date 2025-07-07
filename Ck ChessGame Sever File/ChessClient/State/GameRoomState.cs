using EndoAshu.Chess.Client.Room;
using EndoAshu.Chess.Room;
using Runetide.Util;
using System.Threading;
using System.Threading.Tasks;

namespace EndoAshu.Chess.Client.State
{
    public class GameRoomState : GameState
    {
        public GameRoomState(ChessClient client) : base(client)
        {
        }

        #region RoomQuit
        internal TaskCompletionSource<RoomQuitPacket.QuitStatus>? QuitResponse;
        /// <summary>
        /// 현재 룸에서 나갑니다.
        /// 내부적으로 Task<RoomQuitPacket.StatusCode> QuitRoom(UUID roomId, UUID targetId, int timeoutMS)를 호출합니다.
        /// </summary>
        /// <param name="timeoutMS">요청 timeout</param>
        /// <returns></returns>
        public async Task<RoomQuitPacket.QuitStatus> QuitRoom(int timeoutMS = 1000)
        {
            if (Client.CurrentRoom == null) return RoomQuitPacket.QuitStatus.FAILED;
            if (Client.Account == null) return RoomQuitPacket.QuitStatus.FAILED;
            UUID rid = Client.CurrentRoom.RoomId;
            UUID uid = Client.Account.UniqueId;
            return await QuitRoom(rid, uid, timeoutMS);
        }

        /// <summary>
        /// 현재 룸에 있는 특정 사람을 강퇴시킵니다.
        /// 내부적으로 Task<RoomQuitPacket.StatusCode> QuitRoom(UUID roomId, UUID targetId, int timeoutMS)를 호출합니다.
        /// </summary>
        /// <param name="targetId">강퇴시킬 플레이어의 ID (자신도 가능)</param>
        /// <param name="timeoutMS">요청 timeout</param>
        /// <returns></returns>
        public async Task<RoomQuitPacket.QuitStatus> QuitRoom(UUID targetId, int timeoutMS = 1000)
        {
            if (Client.CurrentRoom == null) return RoomQuitPacket.QuitStatus.FAILED;
            UUID rid = Client.CurrentRoom.RoomId;
            return await QuitRoom(rid, targetId, timeoutMS);
        }

        /// <summary>
        /// 룸에 있는 특정 사람을 강퇴시킵니다.
        /// </summary>
        /// <param name="roomId">룸 ID (요청 플레이어 및 대상 플레이어 모두 이 Room에 없으면 실패합니다.)</param>
        /// <param name="targetId">강퇴시킬 플레이어의 ID (자신도 가능)</param>
        /// <param name="timeoutMS">요청 timeout</param>
        /// <returns></returns>
        public async Task<RoomQuitPacket.QuitStatus> QuitRoom(UUID roomId, UUID targetId, int timeoutMS = 1000)
        {
            if (QuitResponse != null)
                return RoomQuitPacket.QuitStatus.FAILED;

            QuitResponse = new TaskCompletionSource<RoomQuitPacket.QuitStatus>();
            Client.Send(new ClientSideRoomQuitPacket.Request(roomId, targetId));

            RoomQuitPacket.QuitStatus result;
            try
            {
                result = await QuitResponse.Task.WaitAsync(timeoutMS);
            }
            catch
            {
                result = RoomQuitPacket.QuitStatus.TIMEOUT;
            }

            QuitResponse = null;
            return result;
        }

        internal void InternalOnQuitRoom(UUID target, RoomQuitPacket.QuitStatus status)
        {
            if (target == Client.Account?.UniqueId)
            {
                switch (status)
                {
                    case RoomQuitPacket.QuitStatus.SELF_QUIT:
                    case RoomQuitPacket.QuitStatus.KICKED:
                    case RoomQuitPacket.QuitStatus.ROOM_REMOVED:
                        Client.CurrentRoom = null;
                        Client.UpdateState(new GameLobbyState(Client));
                        break;
                }
            }
            QuitResponse?.TrySetResult(status);
        }
        #endregion

        #region RoomRemove
        internal TaskCompletionSource<RoomRemovePacket.RemoveStatus>? RoomRemoveResponse;
        public async Task<RoomRemovePacket.RemoveStatus> RemoveRoom(int timeoutMS = 1000)
        {
            if (RoomRemoveResponse != null)
                return RoomRemovePacket.RemoveStatus.FAILED;

            RoomRemoveResponse = new TaskCompletionSource<RoomRemovePacket.RemoveStatus>();
            Client.Send(new ClientSideRoomRemovePacket.Request());

            RoomRemovePacket.RemoveStatus result;
            try
            {
                result = await RoomRemoveResponse.Task.WaitAsync(timeoutMS);
            }
            catch
            {
                result = RoomRemovePacket.RemoveStatus.TIMEOUT;
            }

            RoomRemoveResponse = null;
            return result;
        }

        internal void InternalOnRemoveRoom(RoomRemovePacket.RemoveStatus status)
        {
            RoomRemoveResponse?.TrySetResult(status);
        }
        #endregion

        #region SetPlayerMode
        /// <summary>
        /// 자신의 PlayerMode를 설정합니다.
        /// </summary>
        /// <param name="mode">설정할 모드</param>
        /// <returns>전송 성공 여부입니다. (반영 여부는 아닙니다.)</returns>
        public bool SetPlayerMode(PlayerMode mode)
        {
            if (Client.CurrentRoom == null) return false;
            if (Client.Account == null) return false;
            UUID uid = Client.Account.UniqueId;
            return SetPlayerMode(uid, mode);
        }

        /// <summary>
        /// PlayerMode를 설정합니다.
        /// </summary>
        /// <param name="target">설정할 대상</param>
        /// <param name="mode">설정할 모드</param>
        /// <returns>전송 성공 여부입니다. (반영 여부는 아닙니다.)</returns>
        public bool SetPlayerMode(UUID target, PlayerMode mode)
        {
            if (target == UUID.NULL) return false;
            var pk = new ClientSideRoomChangePlayerModePacket(target, mode);
            Client.Send(pk);
            return true;
        }
        #endregion

        #region RoomStart
        internal TaskCompletionSource<RoomStartPacket.ResultCode>? StartResponse;

        public async Task<RoomStartPacket.ResultCode> StartRoom(int timeoutMS = 1000)
        {
            if (StartResponse != null)
                return RoomStartPacket.ResultCode.FAILED;

            StartResponse = new TaskCompletionSource<RoomStartPacket.ResultCode>();
            Client.Send(new ClientSideRoomStartPacket.Request());

            RoomStartPacket.ResultCode result;
            try
            {
                result = await StartResponse.Task.WaitAsync(timeoutMS);
            }
            catch
            {
                result = RoomStartPacket.ResultCode.TIMEOUT;
            }

            StartResponse = null;
            return result;
        }

        internal void InternalOnStartRoom(RoomStartPacket.ResultCode result)
        {
            StartResponse?.TrySetResult(result);
        }

        #endregion

        public override void Dispose()
        {
            RoomRemoveResponse?.TrySetCanceled();
            QuitResponse?.TrySetCanceled();
            StartResponse?.TrySetCanceled();
        }
    }
}
