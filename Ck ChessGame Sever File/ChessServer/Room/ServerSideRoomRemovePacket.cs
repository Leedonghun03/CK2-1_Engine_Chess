using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;

namespace EndoAshu.Chess.Server.Room
{
    public sealed class ServerSideRoomRemovePacket
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
                ctx.Get()?.GetAttribute(ChessServer.CHESS_SERVER).IfPresent(server =>
                {
                    ctx.MarkHandle();
                    var status = UserCache.GetIfPresent(ctx.Get()!, cache =>
                    {
                        if (cache.CurrentRoom == null || cache.Account == null)
                            return RoomRemovePacket.RemoveStatus.FAILED;
                        if (!cache.CurrentRoom.IsRoomMaster(cache.Account.UniqueId))
                            return RoomRemovePacket.RemoveStatus.NOT_HOST;
                        return server.Rooms.Remove(cache.CurrentRoom.RoomId) 
                            ? RoomRemovePacket.RemoveStatus.SUCCESS
                            : RoomRemovePacket.RemoveStatus.FAILED;
                    }, RoomRemovePacket.RemoveStatus.FAILED);
                    ctx.Get()!.Send(new Response(status));
                });
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
                throw new System.NotImplementedException();
            }
        }
    }
}
