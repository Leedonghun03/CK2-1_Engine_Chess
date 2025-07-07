using EndoAshu.Chess.InGame.Pieces;
using EndoAshu.Chess.InGame;
using Runetide.Net.Context;
using Runetide.Packet;
using System;
using Runetide.Buffer;

namespace EndoAshu.Chess.Server.InGame
{
    public abstract class ServerSideChessPawnHeldPacket
    {
        public sealed class Request : ChessPawnHeldPacket.Request
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
                    var uuid = pair.Item3;

                    if (data.IsPromote)
                    {
                        context.Get()?.Send(new Response(Commander, -1, -1));
                        return;
                    }

                    var currentPawn = data.Board[TargetX, TargetY];
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

                    data.SetHeld(uuid, TargetX, TargetY);

                    cache.CurrentRoom!.SyncAll(new Response(Commander, TargetX, TargetY));
                });

                if (r < 0)
                {
                    context.Get()?.Send(new Response(Commander, -1, -1));
                }
            }
        }
        public sealed class Response : ChessPawnHeldPacket.Response
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
