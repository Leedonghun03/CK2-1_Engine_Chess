using EndoAshu.Chess.InGame;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Client.InGame
{
    public sealed class ClientSideChessGhostMovePacket : ChessGhostMovePacket
    {
        public ClientSideChessGhostMovePacket(RunetideBuffer buffer) : base(buffer)
        {
        }

        public ClientSideChessGhostMovePacket(UUID id, (float, float, float) position, (float, float, float) velocity) : base(id, position, velocity)
        {
        }

        public ClientSideChessGhostMovePacket(UUID id, (float, float, float) position, (float, float, float) velocity, long timestamp) : base(id, position, velocity, timestamp)
        {
        }

        public override void Handle(PacketContext<NetworkContext> context)
        {
            context.MarkHandle();
        }
    }
}
