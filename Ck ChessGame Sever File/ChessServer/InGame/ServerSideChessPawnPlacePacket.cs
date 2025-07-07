using EndoAshu.Chess.InGame.Pieces;
using EndoAshu.Chess.InGame;
using Runetide.Net.Context;
using Runetide.Packet;
using System;
using Runetide.Buffer;

namespace EndoAshu.Chess.Server.InGame
{
    public abstract class ServerSideChessPawnPlacePacket
    {
        public sealed class Request : ChessPawnPlacePacket.Request
        {
            public Request(ChessPawn.Color commander, int targetX, int targetY) : base(commander, targetX, targetY)
            {
            }

            public Request(RunetideBuffer buf) : base(buf)
            {
            }

            public override void Handle(PacketContext<NetworkContext> context)
            {
                int r = ServerSideChessPawnHelper.GetAccessBoard(context, TargetX, TargetY, (pair, data) =>
                {
                    var ctx = pair.Item1;
                    var cache = pair.Item2;

                    int CurrentX = data.HeldTarget.Item1;
                    int CurrentY = data.HeldTarget.Item2;

                    int DestinationX = TargetX;
                    int DestinationY = TargetY;

                    var currentPawn = data.Board[CurrentX, CurrentY];
                    if (currentPawn == null)
                    {
                        context.Get()?.Send(new Response(Commander, -1, -1));
                        return;
                    }

                    if (currentPawn.PawnColor != data.CurrentTurnColor)
                    {
                        context.Get()?.Send(new Response(Commander, -1, -1));
                        return;
                    }

                    if (!data.Board.IsCanMove(CurrentX, CurrentY, DestinationX, DestinationY))
                    {
                        context.Get()?.Send(new Response(Commander, -1, -1));
                        return;
                    }

                    if (data.IsPromote)
                    {
                        context.Get()?.Send(new Response(Commander, -1, -1));
                        return;
                    }


                    data.SetPlace();

                    data.Board.OnPreMove(CurrentX, CurrentY, DestinationX, DestinationY, out var removed);

                    data.Board[CurrentX, CurrentY] = null;

                    if (data.Board[DestinationX, DestinationY] != null)
                    {
                        var target = data.Board[DestinationX, DestinationY];
                        removed.Add((DestinationX, DestinationY, target.PawnColor, target.PawnType));
                    }

                    data.Board[DestinationX, DestinationY] = currentPawn;

                    data.Board.OnPostMove(CurrentX, CurrentY, DestinationX, DestinationY);

                    cache.CurrentRoom!.Broadcast(e => true, new ServerSideChessPawnRemovedPacket(removed));

                    if (CurrentX != DestinationX || CurrentY != DestinationY)
                    {
                        currentPawn.HasMoved = true;
                        if (
                            currentPawn is Pawn p
                            && (
                                (DestinationY == ChessBoard.CHESS_WHITE_END && currentPawn.PawnColor == ChessPawn.Color.WHITE) 
                                || (DestinationY == ChessBoard.CHESS_BLACK_END && currentPawn.PawnColor == ChessPawn.Color.BLACK)
                                )
                            && !p.IsPromoted)
                        {
                            data.SetPromote(DestinationX, DestinationY, currentPawn.PawnColor);
                        } else {
                            data.NextTurn();
                        }

                        if (currentPawn is Pawn pawnIns)
                        {
                            if (Math.Abs(CurrentY - DestinationY) == 2)
                            {
                                data.enPassantVulnerable = (DestinationX, DestinationY);
                                data.hasEnPassantVulnerable = true;
                            }
                        }

                        if (currentPawn is King king)
                        {
                            if (Math.Abs(CurrentX - DestinationX) == 2)
                            {
                                int dir = Math.Sign(DestinationX - CurrentX);
                                int rookX = dir > 0 ? 7 : 0;
                                int rookY = DestinationY;
                                int newRookX = DestinationX - dir;
                                data.Board[newRookX, rookY] = data.Board[rookX, rookY];
                                data.Board[rookX, rookY] = null;
                            }
                        }
                    }

                    cache.CurrentRoom!.SyncAll(new Response(Commander, DestinationX, DestinationY));
                });

                if (r < 0)
                {
                    context.Get()?.Send(new Response(Commander, -1, -1));
                }
            }
        }
        public sealed class Response : ChessPawnPlacePacket.Response
        {
            public Response(ChessPawn.Color commander, int targetX, int targetY) : base(commander, targetX, targetY)
            {
            }

            public Response(RunetideBuffer buf) : base(buf)
            {
            }

            public override void Handle(PacketContext<NetworkContext> context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
