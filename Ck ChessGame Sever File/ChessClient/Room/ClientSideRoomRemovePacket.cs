using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;

namespace EndoAshu.Chess.Client.Room
{
    public sealed class ClientSideRoomRemovePacket
    {
        public sealed class Request : RoomRemovePacket.Request
        {
            public Request()
            {
            }

            public Request(RunetideBuffer buf) : base(buf)
            {
            }

            public override void Handle(PacketContext<NetworkContext> ctx)
            {
                throw new System.NotSupportedException();
            }
        }

        public sealed class Response : RoomRemovePacket.Response
        {
            public Response(RoomRemovePacket.RemoveStatus status) : base(status)
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
                    (client.State as GameRoomState)?.InternalOnRemoveRoom(Status);
                });
            }
        }
    }
}
