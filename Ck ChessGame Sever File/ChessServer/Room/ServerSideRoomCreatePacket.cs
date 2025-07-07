using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;
using static EndoAshu.Chess.Room.RoomCreatePacket;

namespace EndoAshu.Chess.Server.Room
{
    public sealed class ServerSideRoomCreatePacket
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
                UUID? target = null;
                NetworkContext? net = ctx.Get();
                if (net != null)
                {
                    CreateStatus result = UserCache.GetIfPresent(net, user =>
                    {
                        NetworkContext? net = ctx.Get();
                        if (net == null)
                        {
                            return CreateStatus.FAILED;
                        }

                        return net.GetAttribute(ChessServer.CHESS_SERVER).IfPresent((server) =>
                        {
                            if (user.Account == null)
                            {
                                return CreateStatus.ALREADY_INSIDE;
                            }

                            if (!Options.CheckAllowed())
                            {
                                return CreateStatus.OPTION_NOT_ALLOWED;
                            }

                           ServerRoom? room = server.Rooms.Create(user.Account.UniqueId, Options);

                            if (room != null)
                            {
                                room.Add(user.Ctx!);
                                target = room.RoomId;
                                return CreateStatus.SUCCESS;
                            }
                            return CreateStatus.FAILED;
                        }, CreateStatus.FAILED);
                    }, CreateStatus.FAILED);

                    net.Send(new Response(result, target));
                }
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
                throw new System.NotSupportedException();
            }
        }
    }
}
