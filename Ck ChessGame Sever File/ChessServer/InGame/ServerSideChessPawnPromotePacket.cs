using EndoAshu.Chess.InGame;
using EndoAshu.Chess.InGame.Pieces;
using EndoAshu.Chess.User;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using System;

namespace EndoAshu.Chess.Server.InGame
{
    public abstract class ServerSideChessPawnPromotePacket
    {
        public sealed class Request : ChessPawnPromotePacket.Request
        {
            public Request(ChessPawn.TypeId type) : base(type)
            {
            }

            public Request(RunetideBuffer buffer) : base(buffer)
            {
            }

            public override void Handle(PacketContext<NetworkContext> context)
            {
                int res = ServerSideChessPawnHelper.GetBoard(context, (i1, i2) =>
                {
                    if (!i2.IsPromote)
                    {
                        context.Get()?.Send(new Response(ChessPawn.TypeId.PAWN, ChessPawnPromotePacket.ResponseType.FAILURE));
                        return;
                    }

                    var currentPawn = i2.Board[i2.PromotePos.Item1, i2.PromotePos.Item2];
                    if (currentPawn == null || currentPawn.PawnColor != i2.CurrentTurnColor)
                    {
                        context.Get()?.Send(new Response(ChessPawn.TypeId.PAWN, ChessPawnPromotePacket.ResponseType.FAIL_COLOR_MISMATCH));
                        return;
                    }

                    if (PromoteType == ChessPawn.TypeId.KING)
                    {
                        context.Get()?.Send(new Response(ChessPawn.TypeId.PAWN, ChessPawnPromotePacket.ResponseType.FAIL_PROMOTE_TO_KING));
                        return;
                    }

                    lock (i2.lockObj)
                    {
                        if (!i2.IsPromote)
                        {
                            i1.Item1.Send(new Response(ChessPawn.TypeId.PAWN, ChessPawnPromotePacket.ResponseType.FAILURE));
                        }
                        else
                        {
                            i2.Board[i2.PromotePos.Item1, i2.PromotePos.Item2] = ChessPawn.Create(PromoteType, currentPawn.PawnColor);
                            i2.ClearPromote();
                            i2.NextTurn();
                            i1.Item1.Send(new Response(PromoteType, ChessPawnPromotePacket.ResponseType.SUCCESS));
                        }
                    }

                    i1.Item2.CurrentRoom!.SyncAll();
                });

                if (res < 0)
                {
                    context.Get()?.Send(new Response(ChessPawn.TypeId.PAWN, ChessPawnPromotePacket.ResponseType.FAILURE));
                }
            }
        }

        public sealed class Response : ChessPawnPromotePacket.Response
        {
            public Response(RunetideBuffer buffer) : base(buffer)
            {
            }

            public Response(ChessPawn.TypeId type, ChessPawnPromotePacket.ResponseType result) : base(type, result)
            {
            }

            public override void Handle(PacketContext<NetworkContext> context)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
