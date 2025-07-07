using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Server.Room
{
    public sealed class ServerSideRoomJoinPacket
    {
        public sealed class Request : RoomJoinPacket.Request
        {
            public Request(UUID roomId, string password) : base(roomId, password)
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
                    ServerRoom? room = server.Rooms.GetRoom(RoomId);
                    JoinRoomStatus res;
                    if (room != null)
                    {
                        if (room.Options.Password == null)
                            res = room.Add(ctx.Get()!);
                        else
                        {
                            if (Password == room.Options.Password)
                            {
                                res = room.Add(ctx.Get()!);
                            }
                            else
                                res = JoinRoomStatus.INCORRECT_PASSWORD;
                        }
                    }
                    else
                        res = JoinRoomStatus.ROOM_NOT_FOUND;

                    ctx.Get()!.Send(new ServerSideRoomJoinPacket.Response(res));
                });
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
                throw new System.NotSupportedException();
            }
        }
    }
}
