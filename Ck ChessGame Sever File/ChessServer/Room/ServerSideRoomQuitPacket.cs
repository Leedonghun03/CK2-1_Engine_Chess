using EndoAshu.Chess.Room;
using EndoAshu.Chess.User;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;
using System;

namespace EndoAshu.Chess.Server.Room
{
    public sealed class ServerSideRoomQuitPacket
    {
        public sealed class Response : RoomQuitPacket.Response
        {
            public Response(RunetideBuffer buf) : base(buf)
            {
            }

            public Response(UUID target, RoomQuitPacket.QuitStatus status) : base(target, status)
            {
            }

            public override void Handle(PacketContext<NetworkContext> ctx)
            {
                throw new NotSupportedException();
            }
        }

        public sealed class Request : RoomQuitPacket.Request
        { 
            public Request(RunetideBuffer buf) : base(buf)
            {
            }

            public Request(UUID roomId, UUID targetId) : base(roomId, targetId)
            {
            }

            public override void Handle(PacketContext<NetworkContext> ctx)
            {
                ctx.Get()?.GetAttribute(ChessServer.CHESS_SERVER).IfPresent(server =>
                {
                    ctx.MarkHandle();
                    UUID userUid = ctx.Get()!.GetAttribute(UserAccount.ACCOUNT_KEY).Get()?.UniqueId ?? UUID.NULL;
                    ServerRoom? room = server.Rooms.GetRoom(RoomId);

                    RoomQuitPacket.QuitStatus code = RoomQuitPacket.QuitStatus.FAILED;
                    if (room == null)
                    {
                        code = RoomQuitPacket.QuitStatus.ROOM_REMOVED;
                    }
                    else if (room.HasMember(userUid))
                    {
                        if (TargetId != userUid)
                        {
                            if (!room.IsRoomMaster(userUid))
                            {
                                code = RoomQuitPacket.QuitStatus.NOT_HOST;
                            }
                            else if (room.KickMember(TargetId, true))
                                code = RoomQuitPacket.QuitStatus.TARGET_KICKED;
                        } else
                        {
                            if (room.KickMember(TargetId, false))
                                code = RoomQuitPacket.QuitStatus.SELF_QUIT;
                        }
                    }
                    ctx.Get()!.Send(new ServerSideRoomQuitPacket.Response(TargetId, code));

                    if (room != null)
                        server.Rooms.CheckIfEmpty(room);
                });
            }
        }
    }
}
