using EndoAshu.Chess.InGame.Pieces;
using EndoAshu.Chess.InGame;
using Runetide.Net.Context;
using Runetide.Packet;
using System;
using EndoAshu.Chess.Client.State;
using Runetide.Buffer;

namespace EndoAshu.Chess.Client.InGame
{
    public abstract class ClientSideChessPawnPlacePacket
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
                throw new NotImplementedException();
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

            public override void Handle(PacketContext<NetworkContext> ctx)
            {
                ctx.Get()?.GetAttribute(ChessClient.CHESS_CLIENT).IfPresent(client =>
                {
                    ctx.MarkHandle();
                    (client.State as GameInState)?.InternalOnPawnPlace(this);
                });
            }
        }
    }
}
