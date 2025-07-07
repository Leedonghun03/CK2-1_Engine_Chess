using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;

namespace EndoAshu.Chess.Client.Room
{
    public sealed class ClientSideRoomStartPacket
    {
        public sealed class Request : RoomStartPacket.Request
        {
            public Request()
            {
            }

            public Request(RunetideBuffer buf) : base(buf)
            {
            }

            public override void Handle(PacketContext<NetworkContext> context)
            {
                throw new System.NotImplementedException();
            }
        }

        public sealed class Response : RoomStartPacket.Response
        {
            public Response(RoomStartPacket.ResultCode code) : base(code)
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
                    (client.State as GameRoomState)?.InternalOnStartRoom(Code);
                });
            }
        }
    }
}
