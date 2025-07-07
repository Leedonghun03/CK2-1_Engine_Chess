using Runetide.Buffer;

namespace EndoAshu.Chess.InGame.Pieces
{
    public class Knight : ChessPawn
    {
        public override TypeId PawnType => TypeId.KNIGHT;

        public Knight(Color color) : base(color)
        {

        }

        public Knight(RunetideBuffer buffer) : base(buffer)
        {
        }

        public override void Write(RunetideBuffer buffer)
        {
            base.Write(buffer);
            // Additional Pawn-specific data can be written here
        }
    }
}