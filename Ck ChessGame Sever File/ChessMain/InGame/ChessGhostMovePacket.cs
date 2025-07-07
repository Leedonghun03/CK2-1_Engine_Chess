using Runetide.Buffer;
using Runetide.Packet;
using Runetide.Util;
using System;

namespace EndoAshu.Chess.InGame
{
    public abstract class ChessGhostMovePacket : BasePacket
    {
        public static readonly int PacketId = 0x4000 | 0x7;

        public UUID Id { get; }
        public (float, float, float) Position { get; }
        public (float, float, float) Velocity { get; }
        public long Timestamp { get; }

        public ChessGhostMovePacket(UUID id, (float, float, float) position, (float, float, float) velocity)
            : this(id, position, velocity, (DateTime.UtcNow - DateTime.UnixEpoch).Ticks)
        {
        }

        public ChessGhostMovePacket(UUID id, (float, float, float) position, (float, float, float) velocity, long timestamp)
        {
            Id = id;
            Position = position;
            Velocity = velocity;
            Timestamp = timestamp;
        }

        public ChessGhostMovePacket(RunetideBuffer buffer)
            : this(
                  buffer.ReadUUID(),
                  (buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle()),
                  (buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle()),
                  buffer.ReadInt64()
            )
        {
        }

        public override void Write(RunetideBuffer buffer)
        {
            buffer.WriteUUID(Id);
            buffer.WriteSingle(Position.Item1);
            buffer.WriteSingle(Position.Item2);
            buffer.WriteSingle(Position.Item3);
            buffer.WriteSingle(Velocity.Item1);
            buffer.WriteSingle(Velocity.Item2);
            buffer.WriteSingle(Velocity.Item3);
            buffer.WriteInt64(Timestamp);
        }
    }
}