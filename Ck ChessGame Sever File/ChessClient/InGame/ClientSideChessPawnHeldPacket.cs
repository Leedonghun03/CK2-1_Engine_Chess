using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.InGame;
using EndoAshu.Chess.InGame.Pieces;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using System;
using System.Collections.Generic;
using System.Text;

namespace EndoAshu.Chess.Client.InGame
{
    public abstract class ClientSideChessPawnHeldPacket
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
                throw new NotImplementedException();
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

            public override void Handle(PacketContext<NetworkContext> ctx)
            {
                ctx.Get()?.GetAttribute(ChessClient.CHESS_CLIENT).IfPresent(client =>
                {
                    ctx.MarkHandle();
                    (client.State as GameInState)?.InternalOnPawnHeld(this);
                });
            }
        }
    }
}
