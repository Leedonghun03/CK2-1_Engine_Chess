using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.InGame;
using EndoAshu.Chess.InGame.Pieces;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;

namespace EndoAshu.Chess.Client.InGame
{
    public abstract class ClientSideChessPawnPromotePacket
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
                throw new System.NotImplementedException();
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

            public override void Handle(PacketContext<NetworkContext> ctx)
            {
                ctx.Get()?.GetAttribute(ChessClient.CHESS_CLIENT).IfPresent(client =>
                {
                    ctx.MarkHandle();
                    (client.State as GameInState)?.InternalOnPromote(this);
                });
            }
        }
    }
}
