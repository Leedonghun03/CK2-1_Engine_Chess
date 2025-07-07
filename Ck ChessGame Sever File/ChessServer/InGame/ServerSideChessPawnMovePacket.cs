using EndoAshu.Chess.InGame;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;

namespace EndoAshu.Chess.Server.InGame
{
    public class ServerSideChessPawnMovePacket
    {
        public sealed class Request : ChessPawnMovePacket.Request
        {
            public Request(RunetideBuffer buffer) : base(buffer)
            {
            }

            public Request(int x, int y, int destX, int destY) : base(x, y, destX, destY)
            {
            }

            public override void Handle(PacketContext<NetworkContext> ctx)
            {
                ctx.MarkHandle();
                int res = ServerSideChessPawnHelper.GetAccessBoard(ctx, CurrentX, CurrentY, (pair, data) =>
                {
                    var net = pair.Item1;
                    var cache = pair.Item2;

                    var currentPawn = data.Board[CurrentX, CurrentY];
                    if (currentPawn == null)
                    {
                        net.Send(new Response(ChessPawnMovePacket.ResultCode.INVALID_POSITION));
                        return;
                    }

                    if (currentPawn.PawnColor != data.CurrentTurnColor)
                    {
                        net.Send(new Response(ChessPawnMovePacket.ResultCode.INVALID_PIECE));
                        return;
                    }

                    if (!data.Board.IsCanMove(CurrentX, CurrentY, DestinationX, DestinationY))
                    {
                        net.Send(new Response(ChessPawnMovePacket.ResultCode.INVALID_DESTINATION));
                        return;
                    }

                    data.Board[CurrentX, CurrentY] = null;
                    if (DestinationX >= 0 && DestinationX < 8 && DestinationY >= 0 && DestinationY < 8)
                    {
                        data.Board[DestinationX, DestinationY] = currentPawn;
                    }
                    currentPawn.HasMoved = true;
                    net.Send(new Response(ChessPawnMovePacket.ResultCode.SUCCESS));
                    cache.CurrentRoom!.SyncAll();
                });

                if (res == -2)
                {
                    ctx.Get()?.Send(new Response(ChessPawnMovePacket.ResultCode.INVALID_TURN));
                }
                else if (res < 0)
                {
                    ctx.Get()?.Send(new Response(ChessPawnMovePacket.ResultCode.FAILED));
                }
            }
        }

        public sealed class Response : ChessPawnMovePacket.Response
        {


            public Response(RunetideBuffer buffer) : base(buffer)
            {
            }

            public Response(ChessPawnMovePacket.ResultCode code) : base(code)
            {
            }

            public override void Handle(PacketContext<NetworkContext> context)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}