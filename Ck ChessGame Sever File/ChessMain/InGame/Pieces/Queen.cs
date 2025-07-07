using Runetide.Buffer;

namespace EndoAshu.Chess.InGame.Pieces
{
    public class Queen : ChessPawn
    {
        public override TypeId PawnType => TypeId.QUEEN;

        public Queen(Color color) : base(color)
        {

        }

        public Queen(RunetideBuffer buffer) : base(buffer)
        {
        }

        public override void Write(RunetideBuffer buffer)
        {
            base.Write(buffer);
            // Additional Pawn-specific data can be written here
        }
    }
}