using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Client.Room
{
    public sealed class ClientSideRoomJoinPacket
    {
        public sealed class Request : RoomJoinPacket.Request
        {
            public Request(UUID roomId) : this(roomId, string.Empty)
            {

            }

            public Request(UUID roomId, string password) : base(roomId, password)
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

        public sealed class Response : RoomJoinPacket.Response
        {
            public Response(JoinRoomStatus status) : base(status)
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
                    (client.State as GameLobbyState)?.InternalOnListRoom(Status);
                });
            }
        }
    }
}
