using EndoAshu.Chess.Lobby;
using EndoAshu.Chess.User;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Server.Lobby
{
    public sealed class ServerSideLobbyChatPacket : LobbyChatPacket
    {
        public ServerSideLobbyChatPacket(UUID userUniqueId, string userName, string message) : base(userUniqueId, userName, message)
        {

        }

        public ServerSideLobbyChatPacket(RunetideBuffer buffer) : base(buffer)
        {

        }

        public override void Handle(PacketContext<NetworkContext> context)
        {
            var net = context.Get();
            if (net != null)
            {
                var account = net.GetAttribute(UserAccount.ACCOUNT_KEY).Get();
                if (account != null)
                {
                    ServerSideLobbyChatPacket pk = new ServerSideLobbyChatPacket(
                        account.UniqueId,
                        account.Username,
                        Message
                    );
                    net.GetAttribute(ChessServer.CHESS_SERVER).IfPresent(server =>
                    {
                        server.Socket.SendAll((context) =>
                        {
                            var cache = UserCache.Get(context);
                            return cache != null && cache.CurrentRoom == null;
                        }, pk);
                        //context.MarkHandle();
                    });
                }
            }
        }
    }
}
