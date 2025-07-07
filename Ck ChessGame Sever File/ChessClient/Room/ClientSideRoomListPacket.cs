using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;

namespace EndoAshu.Chess.Client.Room
{
    public sealed class ClientSideRoomListPacket
    {
        public sealed class Request : RoomListPacket.Request
        {
            public Request(RunetideBuffer buf) : base(buf)
            {
            }

            public Request(int page, int itemPerPage) : base(page, itemPerPage)
            {
            }

            public override void Handle(PacketContext<NetworkContext> ctx)
            {
                throw new System.NotSupportedException();
            }
        }

        public sealed class Response : RoomListPacket.Response
        {
            public Response(RoomListPacket.Data data) : base(data)
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
                    (client.State as GameLobbyState)?.InternalOnRoomList(Data);
                });
            }
        }
    }
}
