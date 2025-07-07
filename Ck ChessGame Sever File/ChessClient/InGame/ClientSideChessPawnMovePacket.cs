using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.InGame;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;

namespace EndoAshu.Chess.Client.InGame
{
    public class ClientSideChessPawnMovePacket
    {
        public sealed class Request : ChessPawnMovePacket.Request
        {
            public Request(RunetideBuffer buffer) : base(buffer)
            {
            }

            public Request(int x, int y, int destX, int destY) : base(x, y, destX, destY)
            {
            }

            public override void Handle(PacketContext<NetworkContext> context)
            {
                throw new System.NotImplementedException();
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

            public override void Handle(PacketContext<NetworkContext> ctx)
            {
                ctx.Get()?.GetAttribute(ChessClient.CHESS_CLIENT).IfPresent(client =>
                {
                    ctx.MarkHandle();
                    (client.State as GameInState)?.InternalOnPawnMove(Code);
                });
            }
        }
    }
}
