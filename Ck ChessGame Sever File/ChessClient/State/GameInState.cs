using EndoAshu.Chess.Client.InGame;
using EndoAshu.Chess.InGame;
using EndoAshu.Chess.InGame.Pieces;
using Runetide.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EndoAshu.Chess.Client.State
{
    public class GameInState : GameState
    {
        public GameInState(ChessClient client) : base(client)
        {
        }

        #region MovePawn
        internal TaskCompletionSource<ChessPawnMovePacket.ResultCode>? PawnMoveResponse;
        public async Task<ChessPawnMovePacket.ResultCode> PawnMove(int currentX, int currentY, int destinationX, int destinationY, int timeoutMS = 1000)
        {
            if (PawnMoveResponse != null)
                return ChessPawnMovePacket.ResultCode.FAILED;

            PawnMoveResponse = new TaskCompletionSource<ChessPawnMovePacket.ResultCode>();
            Client.Send(new ClientSideChessPawnMovePacket.Request(currentX, currentY, destinationX, destinationY));

            ChessPawnMovePacket.ResultCode result;
            try
            {
                result = await PawnMoveResponse.Task.WaitAsync(timeoutMS);
            }
            catch
            {
                result = ChessPawnMovePacket.ResultCode.TIMEOUT;
            }
            PawnMoveResponse = null;
            return result;
        }

        internal void InternalOnPawnMove(ChessPawnMovePacket.ResultCode data)
        {
            PawnMoveResponse?.TrySetResult(data);
        }
        #endregion

        #region PawnHeld
        internal TaskCompletionSource<ChessPawnPacket>? PawnHeldResponse;
        public async Task<ChessPawnPacket> PawnHeld(int currentX, int currentY, int timeoutMS = 1000)
        {
            var member = Client.CurrentRoom!.Members.FirstOrDefault(e => e.Id == Client.Account!.UniqueId)!;
            ChessPawn.Color color = member.Mode == Chess.Room.PlayerMode.TEAM1 ? ChessPawn.Color.WHITE : ChessPawn.Color.BLACK;

            if (PawnHeldResponse != null)
                return new ClientSideChessPawnHeldPacket.Response(color, -1, -1);

            PawnHeldResponse = new TaskCompletionSource<ChessPawnPacket>();
            Client.Send(new ClientSideChessPawnHeldPacket.Request(color, currentX, currentY));

            ChessPawnPacket result;
            try
            {
                result = await PawnHeldResponse.Task.WaitAsync(timeoutMS);
            }
            catch
            {
                result = new ClientSideChessPawnHeldPacket.Response(color, -1, -1);
            }
            PawnHeldResponse = null;
            return result;
        }

        internal void InternalOnPawnHeld(ChessPawnPacket response)
        {
            PawnHeldResponse?.SetResult(response);
        }
        #endregion

        #region PawnPlace
        internal TaskCompletionSource<ChessPawnPacket>? PawnPlaceResponse;
        public async Task<ChessPawnPacket> PawnPlace(int currentX, int currentY, int timeoutMS = 1000)
        {
            var member = Client.CurrentRoom!.Members.FirstOrDefault(e => e.Id == Client.Account!.UniqueId)!;
            ChessPawn.Color color = member.Mode == Chess.Room.PlayerMode.TEAM1 ? ChessPawn.Color.WHITE : ChessPawn.Color.BLACK;

            if (PawnPlaceResponse != null)
                return new ClientSideChessPawnPlacePacket.Response(color, -1, -1);

            PawnPlaceResponse = new TaskCompletionSource<ChessPawnPacket>();
            Client.Send(new ClientSideChessPawnPlacePacket.Request(color, currentX, currentY));

            ChessPawnPacket result;
            try
            {
                result = await PawnPlaceResponse.Task.WaitAsync(timeoutMS);
            }
            catch
            {
                result = new ClientSideChessPawnPlacePacket.Response(color, -1, -1);
            }
            PawnPlaceResponse = null;
            return result;
        }

        internal void InternalOnPawnPlace(ChessPawnPacket response)
        {
            PawnPlaceResponse?.SetResult(response);
        }
        #endregion

        #region Promote
        internal TaskCompletionSource<ChessPawnPromotePacket.Response>? PawnPromoteResponse;
        public async Task<ChessPawnPromotePacket.Response> PawnPromote(ChessPawn.TypeId promoteTarget, int timeoutMS = 1000)
        {
            if (PawnPromoteResponse != null)
                return new ClientSideChessPawnPromotePacket.Response(ChessPawn.TypeId.PAWN, ChessPawnPromotePacket.ResponseType.FAILURE);

            PawnPromoteResponse = new TaskCompletionSource<ChessPawnPromotePacket.Response>();
            Client.Send(new ClientSideChessPawnPromotePacket.Request(promoteTarget));
            ChessPawnPromotePacket.Response result;
            try
            {
                result = await PawnPromoteResponse.Task.WaitAsync(timeoutMS);
            }
            catch
            {
                result = new ClientSideChessPawnPromotePacket.Response(ChessPawn.TypeId.PAWN, ChessPawnPromotePacket.ResponseType.TIMEOUT_REQUEST);
            }
            PawnPromoteResponse = null;
            return result;
        }

        internal void InternalOnPromote(ChessPawnPromotePacket.Response response)
        {
            PawnPromoteResponse?.TrySetResult(response);
        }
        #endregion

        public void SendGhostMove((float, float, float) position, (float, float, float) velocity)
        {
            Client.Send(new ClientSideChessGhostMovePacket(UUID.NULL, position, velocity));
        }

        public override void Dispose()
        {
            PawnMoveResponse?.SetCanceled();
            PawnHeldResponse?.SetCanceled();
            PawnPlaceResponse?.SetCanceled();
            PawnPromoteResponse?.SetCanceled();
        }
    }
}
