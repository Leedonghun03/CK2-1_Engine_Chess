using EndoAshu.Chess.User;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;

namespace EndoAshu.Chess.Server.User
{
    public sealed class ServerSideUserAccountSyncPacket : UserAccountSyncPacket
    {
        public ServerSideUserAccountSyncPacket(UserAccount? account) : base(account)
        {
        }

        public ServerSideUserAccountSyncPacket(RunetideBuffer buf) : base(buf)
        {
        }

        public override void Handle(PacketContext<NetworkContext> context)
        {

        }
    }
}
