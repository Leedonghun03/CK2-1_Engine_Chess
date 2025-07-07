using EndoAshu.Chess.InGame.Pieces;
using Runetide.Buffer;
using Runetide.Packet;

namespace EndoAshu.Chess.InGame
{
    public abstract class ChessPawnPacket : BasePacket
    {
        public ChessPawn.Color Commander { get; }
        public int TargetX { get; }
        public int TargetY { get; }

        public ChessPawnPacket(ChessPawn.Color commander, int targetX, int targetY)
        {
            Commander = commander;
            TargetX = targetX;
            TargetY = targetY;
        }

        public ChessPawnPacket(RunetideBuffer buffer)
        {
            Commander = buffer.ReadEnum<ChessPawn.Color>();
            TargetX = buffer.ReadInt32();
            TargetY = buffer.ReadInt32();
        }

        public override void Write(RunetideBuffer buffer)
        {
            buffer.WriteEnum(Commander);
            buffer.WriteInt32(TargetX);
            buffer.WriteInt32(TargetY);
        }
    }
}
