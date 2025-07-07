using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Client.Room
{
    public sealed class ClientSideRoomCreatePacket
    {
        public sealed class Request : RoomCreatePacket.Request
        {
            public Request(RoomOptions options) : base(options)
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

        public sealed class Response : RoomCreatePacket.Response
        {
            public Response(RunetideBuffer buf) : base(buf)
            {
            }

            public Response(RoomCreatePacket.CreateStatus status, UUID? roomId) : base(status, roomId)
            {
            }

            public override void Handle(PacketContext<NetworkContext> ctx)
            {
                ctx.Get()?.GetAttribute(ChessClient.CHESS_CLIENT).IfPresent(client =>
                {
                    ctx.MarkHandle();
                    (client.State as GameLobbyState)?.InternalOnCreateRoom(Status, RoomId);
                });
            }
        }
    }
}
