using EndoAshu.Chess.User;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;

namespace EndoAshu.Chess.Client.User
{
    public class ClientSideUserAccountSyncPacket : UserAccountSyncPacket
    {
        public ClientSideUserAccountSyncPacket(UserAccount? account) : base(account)
        {
        }

        public ClientSideUserAccountSyncPacket(RunetideBuffer buf) : base(buf)
        {
        }

        public override void Handle(PacketContext<NetworkContext> ctx)
        {
            NetworkContext? net = ctx.Get();
            if (net != null)
            {
                ctx.MarkHandle();
                net.GetAttribute(UserAccount.ACCOUNT_KEY).Set(new ClientUserAccount(this.AccountData!));
            }
        }
    }
}
