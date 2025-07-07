using EndoAshu.Chess.InGame.Pieces;
using Runetide.Buffer;
using Runetide.Util;
using System;
using System.Collections.Generic;

namespace EndoAshu.Chess.InGame
{
    public class ChessGamePlayingData
    {
        public enum Turn
        {
            WHITE,
            BLACK
        }

        public bool IsPlaying { get; set; } = false;
        public Turn CurrentTurn { get; set; } = Turn.WHITE;
        public int TurnNo { get; private set; } = 1;
        public UUID HeldWho { get; set; } = UUID.NULL;
        public (int, int) HeldTarget { get; set; } = (-1, -1);
        public bool IsHeld => HeldTarget.Item1 != -1 && HeldTarget.Item2 != -1;
        public ChessPawn.Color CurrentTurnColor => CurrentTurn == Turn.WHITE ? ChessPawn.Color.WHITE : ChessPawn.Color.BLACK;

        public ChessBoard Board { get; } = new ChessBoard();

        /// <summary>
        /// 해당 필드는 동기화 되지 않지만, 대입 연산을 통해 대입되었을 경우 차이를 알리기 위함.
        /// 만약, 이 값이 바뀌면 Instance 통째로 덮어씌워진 것임.
        /// </summary>
        public UUID InstanceId { get; private set; } = UUID.Create();

        //게임이 시작될 때 캐싱 (나가도 변동 X)
        public List<UUID> Team1UUID { get; set; } = new List<UUID>();
        //게임이 시작될 때 캐싱 (나가도 변동 X)
        public List<UUID> Team2UUID { get; set; } = new List<UUID>();

        public ChessPawn.Color? PromoteColor { get; set; } = null;
        public (int, int) PromotePos = (-1, -1);
        public DateTime PromoteRequestTime { get; set; } = DateTime.MinValue;

        public (int, int) enPassantVulnerable = (-1, -1);
        public bool hasEnPassantVulnerable = false;

        /// <summary>
        /// 웬만해선 사용하지 마십시오. (Server-Side용)
        /// </summary>
        public readonly object lockObj = new object();

        public void Reset()
        {
            IsPlaying = false;
            Team1UUID.Clear();
            Team2UUID.Clear();
            HeldTarget = (-1, -1);
            CurrentTurn = Turn.WHITE;
            Board.ResetDefault();
            MarkDirty();
        }

        public ChessGamePlayingData()
        {

        }

        public ChessGamePlayingData(RunetideBuffer buffer)
        {
            Read(buffer);
        }

        public void SetHeld(UUID id, int x, int y)
        {
            HeldWho = id;
            HeldTarget = (x, y);
        }

        public void SetPlace()
        {
            HeldWho = UUID.NULL;
            HeldTarget = (-1, -1);
        }

        public void MarkDirty()
        {
            InstanceId = UUID.Create();
        }

        public void Read(RunetideBuffer buffer)
        {
            IsPlaying = buffer.ReadBool();
            CurrentTurn = buffer.ReadEnum<Turn>();
            Board.Read(buffer);

            HeldWho = buffer.ReadUUID();
            int heldX = buffer.ReadInt32();
            int heldY = buffer.ReadInt32();
            HeldTarget = (heldX, heldY);

            TurnNo = buffer.ReadInt32();

            Team1UUID.Clear();
            int t1c = buffer.ReadInt32();
            for (int i = 0; i < t1c; ++i)
                Team1UUID.Add(buffer.ReadUUID());

            Team2UUID.Clear();
            int t2c = buffer.ReadInt32();
            for (int i = 0; i < t2c; ++i)
                Team2UUID.Add(buffer.ReadUUID());

            int promoteX = buffer.ReadInt32();
            int promoteY = buffer.ReadInt32();
            PromotePos = (promoteX, promoteY);
            if (buffer.ReadBool())
                PromoteColor = buffer.ReadEnum<ChessPawn.Color>();
            else
                PromoteColor = null;

            PromoteRequestTime = new DateTime(buffer.ReadInt64(), DateTimeKind.Utc);

            enPassantVulnerable = (buffer.ReadInt32(), buffer.ReadInt32());
            hasEnPassantVulnerable = buffer.ReadBool();

            MarkDirty();
        }

        public void Write(RunetideBuffer buffer)
        {
            buffer.WriteBool(IsPlaying);
            buffer.WriteEnum(CurrentTurn);
            Board.Write(buffer);

            buffer.WriteUUID(HeldWho);
            buffer.WriteInt32(HeldTarget.Item1);
            buffer.WriteInt32(HeldTarget.Item2);

            buffer.WriteInt32(TurnNo);

            buffer.WriteInt32(Team1UUID.Count);
            foreach (var uuid in Team1UUID)
            {
                buffer.WriteUUID(uuid);
            }

            buffer.WriteInt32(Team2UUID.Count);
            foreach (var uuid in Team2UUID)
            {
                buffer.WriteUUID(uuid);
            }

            buffer.WriteInt32(PromotePos.Item1);
            buffer.WriteInt32(PromotePos.Item2);
            buffer.WriteBool(PromoteColor != null);
            if (PromoteColor != null)
                buffer.WriteEnum((ChessPawn.Color)PromoteColor);
            buffer.WriteInt64(PromoteRequestTime.Ticks);

            buffer.WriteInt32(enPassantVulnerable.Item1);
            buffer.WriteInt32(enPassantVulnerable.Item2);
            buffer.WriteBool(hasEnPassantVulnerable);
        }

        public void SetPromote(int destinationX, int destinationY, ChessPawn.Color pawnColor)
        {
            PromoteColor = pawnColor;
            PromotePos = (destinationX, destinationY);
            PromoteRequestTime = DateTime.UtcNow;
        }

        public void ClearPromote()
        {
            PromoteColor = null;
            PromotePos = (-1, -1);
            PromoteRequestTime = DateTime.MinValue;
        }

        public bool IsPromote => PromoteColor != null
            && PromotePos.Item1 >= 0 && PromotePos.Item2 >= 0
            && PromotePos.Item1 < 8 && PromotePos.Item2 < 8;

        public void NextTurn()
        {
            CurrentTurn = CurrentTurn == Turn.WHITE
                ? Turn.BLACK
                : Turn.WHITE;

            hasEnPassantVulnerable = false;
            enPassantVulnerable = (-1, -1);

            TurnNo++;
        }
    }
}
