using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Packet;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EndoAshu.Chess.InGame
{
    public abstract class ChessGameEndPacket : BasePacket
    {
        public static readonly int PacketId = 0x4000 | 0xb;

        public PlayerMode WinnerTeam { get; }
        public ReadOnlyCollection<RoomSyncPacket.SyncMember> Team1 { get; }
        public ReadOnlyCollection<RoomSyncPacket.SyncMember> Team2 { get; }

        public ChessGameEndPacket(PlayerMode winnerTeam, List<RoomSyncPacket.SyncMember> team1, List<RoomSyncPacket.SyncMember> team2)
        {
            WinnerTeam = winnerTeam;
            Team1 = team1.AsReadOnly();
            Team2 = team2.AsReadOnly();
        }

        public ChessGameEndPacket(RunetideBuffer buffer)
        {
            WinnerTeam = buffer.ReadEnum<PlayerMode>();

            int team1Count = buffer.ReadInt32();
            List<RoomSyncPacket.SyncMember> team1 = new List<RoomSyncPacket.SyncMember>(team1Count);
            for (int i = 0; i < team1Count; i++)
            {
                team1.Add(new RoomSyncPacket.SyncMember(buffer));
            }
            Team1 = team1.AsReadOnly();

            int team2Count = buffer.ReadInt32();
            List<RoomSyncPacket.SyncMember> team2 = new List<RoomSyncPacket.SyncMember>(team2Count);
            for (int i = 0; i < team2Count; i++)
            {
                team2.Add(new RoomSyncPacket.SyncMember(buffer));
            }
            Team2 = team2.AsReadOnly();
        }

        public override void Write(RunetideBuffer buffer)
        {
            buffer.WriteEnum(WinnerTeam);

            buffer.WriteInt32(Team1.Count);
            foreach (var member in Team1)
            {
                member.WriteTo(buffer);
            }

            buffer.WriteInt32(Team2.Count);
            foreach (var member in Team2)
            {
                member.WriteTo(buffer);
            }
        }
    }
}
