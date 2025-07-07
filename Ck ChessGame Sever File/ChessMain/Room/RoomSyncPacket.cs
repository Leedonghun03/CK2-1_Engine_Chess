using EndoAshu.Chess.InGame;
using Runetide.Buffer;
using Runetide.Packet;
using Runetide.Util;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace EndoAshu.Chess.Room
{
    /// <summary>
    /// 룸 정보 동기화 패킷.
    /// Server -> Client Only 패킷
    /// </summary>
    public abstract class RoomSyncPacket : BasePacket
    {
        public class SyncMember
        {
            public UUID Id { get; }
            public string Name { get; }
            public PlayerMode Mode { get; }

            public SyncMember(UUID id, string name, PlayerMode mode)
            {
                Id = id;
                Name = name;
                Mode = mode;
            }

            public SyncMember(RunetideBuffer buf)
            {
                Id = buf.ReadUUID();
                Name = buf.ReadString();
                Mode = buf.ReadEnum<PlayerMode>();
            }

            public void WriteTo(RunetideBuffer buf)
            {
                buf.WriteUUID(Id);
                buf.WriteString(Name);
                buf.WriteEnum(Mode);
            }
        }

        public static readonly int PacketId = 0x2000 | 0x1;

        public UUID RoomId { get; }
        public UUID RoomMasterId { get; }
        public RoomOptions Options { get; }
        public ReadOnlyCollection<SyncMember> Members { get; }
        public ChessGamePlayingData PlayingData { get; }

        public RoomSyncPacket(AbstractRoom room)
        {
            RoomId = room.RoomId;
            RoomMasterId = room.RoomMasterId;
            Options = room.Options;
            Members = GetMembers(room).AsReadOnly();
            PlayingData = room.PlayingData;
        }

        public abstract List<SyncMember> GetMembers(AbstractRoom room);

        public RoomSyncPacket(RunetideBuffer buf) : base(buf)
        {
            RoomId = buf.ReadUUID();
            RoomMasterId = buf.ReadUUID();
            Options = buf.ReadJson<RoomOptions>()!;

            List<SyncMember> members = new List<SyncMember>();
            int memberCount = buf.ReadInt32();
            for(int i = 0; i < memberCount; ++i)
            {
                members.Add(new SyncMember(buf));
            }
            Members = members.AsReadOnly();
            PlayingData = new ChessGamePlayingData(buf);
        }

        public override void Write(RunetideBuffer buf)
        {
            buf.WriteUUID(RoomId);
            buf.WriteUUID(RoomMasterId);
            buf.WriteJson(Options);
            buf.WriteInt32(Members.Count);
            foreach (var member in Members)
            {
                member.WriteTo(buf);
            }
            PlayingData.Write(buf);
        }
    }
}