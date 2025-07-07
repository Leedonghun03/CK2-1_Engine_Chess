using EndoAshu.Chess.InGame.Pieces;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;

namespace EndoAshu.Chess.InGame
{
    public abstract class ChessPawnHeldPacket
    {
        public abstract class Request : ChessPawnPacket
        {
            public static readonly int PacketId = 0x4000 | 0x3;

            public Request(ChessPawn.Color commander, int targetX, int targetY) : base(commander, targetX, targetY)
            {
            }

            public Request(RunetideBuffer buf) : base(buf)
            {
            }
        }

        public abstract class Response : ChessPawnPacket
        {
            public static readonly int PacketId = 0x4000 | 0x4;
            public Response(ChessPawn.Color commander, int targetX, int targetY) : base(commander, targetX, targetY)
            {
            }
            public Response(RunetideBuffer buf) : base(buf)
            {
            }
        }
    }
}
