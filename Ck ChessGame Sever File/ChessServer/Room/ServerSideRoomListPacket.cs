using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using System.Collections.ObjectModel;
using System.Linq;

namespace EndoAshu.Chess.Server.Room
{
    public sealed class ServerSideRoomListPacket
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
                ctx.Get()?.GetAttribute(ChessServer.CHESS_SERVER).IfPresent(server =>
                {
                    var rooms = server.Rooms.GetRooms(Page, ItemPerPage);
                    var list = rooms.Select(e => new RoomListPacket.Item(e.RoomId, e.Options.Name, e.GetMember(e.RoomMasterId)?.Username ?? "<none>")).ToList().AsReadOnly();
                    var pk = new Response(Page, list, server.Rooms.Count);
                    ctx.Get()!.Send(pk);
                });
            }
        }

        public sealed class Response : RoomListPacket.Response
        {
            public Response(int page, ReadOnlyCollection<RoomListPacket.Item> items, int totalPages) : base(new RoomListPacket.Data(page, items, totalPages))
            {
            }

            public Response(RunetideBuffer buf) : base(buf)
            {
            }

            public override void Handle(PacketContext<NetworkContext> ctx)
            {
                throw new System.NotSupportedException();
            }
        }
    }
}
