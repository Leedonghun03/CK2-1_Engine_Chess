using EndoAshu.Chess.InGame.Pieces;
using Runetide.Buffer;
using System;
using System.Collections.Generic;
using System.Drawing;
using static EndoAshu.Chess.InGame.ChessGamePlayingData;

namespace EndoAshu.Chess.InGame
{
    public class ChessBoard
    {
        public static readonly int CHESS_WHITE_END = 7;
        public static readonly int CHESS_BLACK_END = 0;

        private ChessPawn?[,] Board = new ChessPawn[8, 8];

        public ChessPawn? this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= 8 || y < 0 || y >= 8)
                    return null;
                return Board[x, y];
            }
            set => Board[x, y] = value;
        }

        public ChessBoard()
        {
            Reset();
        }

        public void Reset()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Board[i, j] = null;
                }
            }
        }

        public void ResetDefault()
        {
            Reset();
            for(int i = 0; i < 8; ++i)
            {
                Board[i, 1] = new Pawn(ChessPawn.Color.WHITE);
                Board[i, 6] = new Pawn(ChessPawn.Color.BLACK);
            }
            Board[0, 0] = new Rook(ChessPawn.Color.WHITE);
            Board[7, 0] = new Rook(ChessPawn.Color.WHITE);
            Board[0, 7] = new Rook(ChessPawn.Color.BLACK);
            Board[7, 7] = new Rook(ChessPawn.Color.BLACK);

            Board[1, 0] = new Knight(ChessPawn.Color.WHITE);
            Board[6, 0] = new Knight(ChessPawn.Color.WHITE);
            Board[1, 7] = new Knight(ChessPawn.Color.BLACK);
            Board[6, 7] = new Knight(ChessPawn.Color.BLACK);

            Board[2, 0] = new Bishop(ChessPawn.Color.WHITE);
            Board[5, 0] = new Bishop(ChessPawn.Color.WHITE);
            Board[2, 7] = new Bishop(ChessPawn.Color.BLACK);
            Board[5, 7] = new Bishop(ChessPawn.Color.BLACK);


            Board[3, 0] = new Queen(ChessPawn.Color.WHITE);
            Board[4, 0] = new King(ChessPawn.Color.WHITE);
            Board[3, 7] = new Queen(ChessPawn.Color.BLACK);
            Board[4, 7] = new King(ChessPawn.Color.BLACK);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var p = Board[i, j];
                    if (p != null)
                    {
                        p.BeforePosition = (i, j);
                        p.HasMoved = false;
                    }
                }
            }
        }

        public void Read(RunetideBuffer buffer)
        {
            Reset();
            int count = buffer.ReadInt32();
            for(int i = 0; i < count; ++i)
            {
                int x = buffer.ReadInt32();
                int y = buffer.ReadInt32();
                ChessPawn.TypeId id = buffer.ReadEnum<ChessPawn.TypeId>();
                ChessPawn pawn = ChessPawn.Create(id, buffer);
                if (pawn != null)
                {
                    Board[x, y] = pawn;
                }
            }
        }

        public void Write(RunetideBuffer buffer)
        {
            int count = 0;
            using (RunetideBuffer cache = BufferAllocator.GetAllocator().Allocate())
            {
                for (int x = 0; x < 8; ++x)
                {
                    for (int y = 0; y < 8; ++y)
                    {
                        ChessPawn? pawn = Board[x, y];
                        if (pawn != null)
                        {
                            ++count;
                            cache.WriteInt32(x);
                            cache.WriteInt32(y);
                            cache.WriteEnum(pawn.PawnType);
                            pawn.Write(cache);
                        }
                    }
                }
                buffer.WriteInt32(count);
                if (count > 0)
                    buffer.Write(cache.ReadToEnd());
            }
        }

        public bool IsCanMove(int currentX, int currentY, int destinationX, int destinationY)
        {
            //Server-Side 검증로직 구현 X (Client-Side에서 계산함, 치트 유저 고려 X)
            return true;
        }

        public void OnPostMove(int currentX, int currentY, int destinationX, int destinationY)
        {
            if (Board[destinationX, destinationY] is Pawn pawn)
            {
                pawn.IsTwiceMoved = Math.Abs(currentY - destinationY) == 2;
            }
        }

        public void OnPreMove(int currentX, int currentY, int destinationX, int destinationY, out List<(int, int, ChessPawn.Color, ChessPawn.TypeId)> preRemoved)
        {
            preRemoved = new List<(int, int, ChessPawn.Color, ChessPawn.TypeId)>();

            //01. En Passant
            if (Board[currentX, currentY] is Pawn pawn && pawn.PawnColor == ChessPawn.Color.WHITE)
            {
                if (currentY == 4 && destinationY == 5 && Math.Abs(currentX - destinationX) == 1
                    && Board[destinationX, 4] is Pawn p2 && p2.IsTwiceMoved)
                {
                    // En Passant 가능
                    Board[destinationX, 4] = null;
                    preRemoved.Add((destinationX, 4, p2.PawnColor, p2.PawnType));
                }
            }
            else if (Board[currentX, currentY] is Pawn blackPawn && blackPawn.PawnColor == ChessPawn.Color.BLACK)
            {
                if (currentY == 3 && destinationY == 2 && Math.Abs(currentX - destinationX) == 1
                    && Board[destinationX, 3] is Pawn p2 && p2.IsTwiceMoved)
                {
                    // En Passant 가능
                    Board[destinationX, 3] = null;
                    preRemoved.Add((destinationX, 3, p2.PawnColor, p2.PawnType));
                }
            }
        }
    }
}
