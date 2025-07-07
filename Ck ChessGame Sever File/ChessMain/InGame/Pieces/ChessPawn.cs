using Runetide.Buffer;
using System;

namespace EndoAshu.Chess.InGame.Pieces
{
    public abstract class ChessPawn
    {
        public enum Color
        {
            WHITE,
            BLACK
        }

        public enum TypeId
        {
            PAWN,
            KNIGHT,
            ROOK,
            BISHOP,
            QUEEN,
            KING
        }

        public Color PawnColor { get; }

        public abstract TypeId PawnType { get; }
        public bool HasMoved { get; set; } = false;

        public (int, int) BeforePosition { get; set; } = (-1, -1);

        public ChessPawn(Color pawnColor)
        {
            PawnColor = pawnColor;
        }

        public ChessPawn(RunetideBuffer buffer)
        {
            PawnColor = buffer.ReadEnum<Color>();
            HasMoved = buffer.ReadBool();
            int bx = buffer.ReadInt32();
            int by = buffer.ReadInt32();
            BeforePosition = (bx, by);
        }

        public virtual void Write(RunetideBuffer buffer)
        {
            buffer.WriteEnum(PawnColor);
            buffer.WriteBool(HasMoved);
            buffer.WriteInt32(BeforePosition.Item1);
            buffer.WriteInt32(BeforePosition.Item2);
        }

        public static ChessPawn Create(TypeId id, RunetideBuffer buffer)
        {
            return id switch
            {
                TypeId.PAWN => new Pawn(buffer),
                TypeId.KNIGHT => new Knight(buffer),
                TypeId.ROOK => new Rook(buffer),
                TypeId.BISHOP => new Bishop(buffer),
                TypeId.QUEEN => new Queen(buffer),
                TypeId.KING => new King(buffer),
                _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
            };
        }

        public static ChessPawn Create(TypeId id, ChessPawn.Color color)
        {
            return id switch
            {
                TypeId.PAWN => new Pawn(color),
                TypeId.KNIGHT => new Knight(color),
                TypeId.ROOK => new Rook(color),
                TypeId.BISHOP => new Bishop(color),
                TypeId.QUEEN => new Queen(color),
                TypeId.KING => new King(color),
                _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
            };
        }
    }
}
