using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.Lobby;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Client.Lobby
{
    public class ClientSideLobbyChatPacket : LobbyChatPacket
    {
        public ClientSideLobbyChatPacket(string message) : base(UUID.NULL, "<CLIENTSEND>", message)
        {
        }

        public ClientSideLobbyChatPacket(RunetideBuffer buffer) : base(buffer)
        {
        }

        public override void Handle(PacketContext<NetworkContext> context)
        {
            context.Get()?.GetAttribute(ChessClient.CHESS_CLIENT).IfPresent(client =>
            {
                var ls = client.State as GameLobbyState;
                if (ls != null)
                {
                    context.MarkHandle();
                    context.EnqueueAction(() =>
                    {
                        ls.OnReceiveChat(SenderId, SenderName, Message);
                    });
                }
            });
        }
    }
}
