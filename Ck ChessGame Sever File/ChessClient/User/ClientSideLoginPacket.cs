using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.User;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;

namespace EndoAshu.Chess.Client.User
{
    public sealed class ClientSideLoginPacket : LoginPacket
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
                NetworkContext? net = ctx.Get();
                if (net != null)
                {
                    ctx.MarkHandle();
                    if (Result != LoginStatus.FAILED)
                    {
                        net.GetAttribute(UserAccount.ACCOUNT_KEY).Set(new ClientUserAccount(this.AccountData!));
                    }
                    else
                    {
                        net.GetAttribute(UserAccount.ACCOUNT_KEY).Remove();
                    }
                    (net.GetAttribute(ChessClient.CHESS_CLIENT).Get()?.State as GameLoginState)?.LoginResponse?.SetResult(this);
                }
            }
        }

        public ClientSideLoginPacket(RunetideBuffer buf) : base(buf)
        {
        }

        public ClientSideLoginPacket(string id, string password) : base(id, password, ChessProtocolVersion.PROTOCOL_VERSION)
        {
        }

        public override void Handle(PacketContext<NetworkContext> ctx)
        {
            throw new System.NotSupportedException();
        }
    }
}
