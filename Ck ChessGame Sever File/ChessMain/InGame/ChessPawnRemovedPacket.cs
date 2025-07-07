using EndoAshu.Chess.InGame.Pieces;
using Runetide.Buffer;
using Runetide.Packet;
using System.Collections.Generic;

namespace EndoAshu.Chess.InGame
{
    public abstract class ChessPawnRemovedPacket : BasePacket
    {
        public static readonly int PacketId = 0x4000 | 0xa;

        public List<(int, int, ChessPawn.Color, ChessPawn.TypeId)> RemovedPositions { get; }

        public ChessPawnRemovedPacket(List<(int, int, ChessPawn.Color, ChessPawn.TypeId)> removed)
        {
            RemovedPositions = removed;
        }

        public ChessPawnRemovedPacket(RunetideBuffer buffer) : base(buffer)
        {
            RemovedPositions = new List<(int, int, ChessPawn.Color, ChessPawn.TypeId)>();
            int count = buffer.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                RemovedPositions.Add((buffer.ReadInt32(), buffer.ReadInt32(), buffer.ReadEnum<ChessPawn.Color>(), buffer.ReadEnum<ChessPawn.TypeId>()));
            }
        }

        public override void Write(RunetideBuffer buffer)
        {
            buffer.WriteInt32(RemovedPositions.Count);
            foreach (var pos in RemovedPositions)
            {
                buffer.WriteInt32(pos.Item1);
                buffer.WriteInt32(pos.Item2);
                buffer.WriteEnum(pos.Item3);
                buffer.WriteEnum(pos.Item4);
            }
        }
    }
}
