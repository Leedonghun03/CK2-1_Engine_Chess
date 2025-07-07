using EndoAshu.Chess.User;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using System;

namespace EndoAshu.Chess.Server.User
{
    public sealed class ServerSideLoginPacket : LoginPacket
    {
        public sealed new class Response : LoginPacket.Response
        {
            public Response(RunetideBuffer buf) : base(buf)
            {
            }

            public Response(LoginStatus status, UserAccount? account) : base(status, account)
            {
            }

            public override void Handle(PacketContext<NetworkContext> ctx)
            {
                throw new System.NotSupportedException();
            }
        }

        public ServerSideLoginPacket(RunetideBuffer buf) : base(buf)
        {
        }

        public ServerSideLoginPacket(string id, string password) : base(id, password, -1)
        {
        }

        public override void Handle(PacketContext<NetworkContext> ctx)
        {
            NetworkContext? net = ctx.Get();

            if (net != null)
            {
                ctx.MarkHandle();
                if (clientProtocolVer != ChessProtocolVersion.PROTOCOL_VERSION)
                {
                    net.Send(new Response(LoginPacket.LoginStatus.PROTOCOL_MISMATCH, null));
                } else if (net.GetAttribute(UserAccount.ACCOUNT_KEY).Get() is ServerUserAccount account)
                {
                    net.Send(new Response(LoginPacket.LoginStatus.ALREADY_LOGINED, account));
                }
                else
                {
                    var res = ServerServices.GetService<IUserAccountService>().Login(Id, Password);
                    var server = net.GetAttribute(ChessServer.CHESS_SERVER).Get();

                    if (res != null && server != null)
                    {
                        ctx.Get()!.Logger?.Info($"[AUTH] Login User : {res.UniqueId} {res.Username}");
                        ctx.Get()!.GetAttribute(UserAccount.ACCOUNT_KEY).Set(res);

                        var cache = server.GetOrCreateUserCache(res, net);
                        UserCache.Apply(net, cache);
                        net.Send(new Response(LoginPacket.LoginStatus.SUCCESS, res));
                    }
                    else
                    {
                        ctx.Get()!.Logger?.Info($"[AUTH] Login Failed : {res} {server}");
                        net.Send(new Response(LoginPacket.LoginStatus.FAILED, null));
                    }
                }
            }
        }
    }
}
