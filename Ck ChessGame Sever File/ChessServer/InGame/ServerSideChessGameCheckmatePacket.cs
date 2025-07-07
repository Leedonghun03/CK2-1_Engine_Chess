using EndoAshu.Chess.InGame;
using EndoAshu.Chess.InGame.Pieces;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;

namespace EndoAshu.Chess.Server.InGame
{
    public class ServerSideChessGameCheckmatePacket : ChessGameCheckmatePacket
    {
        public ServerSideChessGameCheckmatePacket(RunetideBuffer buffer) : base(buffer)
        {
        }

        public ServerSideChessGameCheckmatePacket(ResultCode code, ChessPawn.Color requestColor) : base(code, requestColor)
        {
        }

        public override void Handle(PacketContext<NetworkContext> context)
        {
            int res = ServerSideChessPawnHelper.GetBoard(context, (i1, i2) =>
            {
                lock (i2.lockObj)
                {
                    if (i2.IsPlaying)
                    {
                        if (Code == ResultCode.STALEMATE)
                            i1.Item2.CurrentRoom!.OnWin(Chess.Room.PlayerMode.DRAW);
                        else
                            i1.Item2.CurrentRoom!.OnWin(RequestColor == ChessPawn.Color.WHITE ? Chess.Room.PlayerMode.TEAM1 : Chess.Room.PlayerMode.TEAM2);

                        i1.Item2.CurrentRoom!.SyncAll();
                    }
                }
            });
        }
    }
}
