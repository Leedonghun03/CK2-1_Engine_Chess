using EndoAshu.Chess.Room;
using EndoAshu.Chess.User;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Server.Room
{
    public sealed class ServerSideRoomStartPacket { 
    public sealed class Request : RoomStartPacket.Request
    {
        public Request()
        {
        }

        public Request(RunetideBuffer buf) : base(buf)
        {
        }

        public override void Handle(PacketContext<NetworkContext> ctx)
        {
            NetworkContext? net = ctx.Get();
            net?.GetAttribute(ChessServer.CHESS_SERVER).IfPresent(server =>
            {
                ctx.MarkHandle();
                UUID userUid = ctx.Get()!.GetAttribute(UserAccount.ACCOUNT_KEY).Get()?.UniqueId ?? UUID.NULL;
                UserCache.GetIfPresent(ctx.Get()!, cache =>
                {
                    if (cache.CurrentRoom != null)
                    {
                        if (!cache.CurrentRoom.IsRoomMaster(userUid))
                        {
                            net.Send(new Response(RoomStartPacket.ResultCode.NOT_HOST));
                            return;
                        }
                        if (cache.CurrentRoom.IsCanStartAble())
                        {
                            if (cache.CurrentRoom.Start(false))
                            {
                                net.Send(new Response(RoomStartPacket.ResultCode.SUCCESS));
                                cache.CurrentRoom.SyncAll();
                            } else
                            {
                                net.Send(new Response(RoomStartPacket.ResultCode.FAILED));
                            }
                        }
                        else
                        {
                            net.Send(new Response(RoomStartPacket.ResultCode.FAILED));
                        }
                    }
                });
            });
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

            public override void Handle(PacketContext<NetworkContext> context)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
