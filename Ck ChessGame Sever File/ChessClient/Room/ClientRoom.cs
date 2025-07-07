using EndoAshu.Chess.Room;
using System;
using System.Collections.Generic;
using static EndoAshu.Chess.Room.RoomSyncPacket;
using System.Collections.ObjectModel;
using System.Linq;
using Runetide.Util;

namespace EndoAshu.Chess.Client.Room
{
    public class ClientRoom : AbstractRoom
    {
        public override RoomState State => throw new System.NotImplementedException();
        public ReadOnlyCollection<SyncMember> Members { get; set; } = new List<SyncMember>().AsReadOnly();

        public ClientRoom()
        {
        }

        internal void HandleSync(ClientSideRoomSyncPacket pk)
        {
            Options = pk.Options;
            RoomId = pk.RoomId;
            RoomMasterId = pk.RoomMasterId;
            Members = pk.Members;
            PlayingData = pk.PlayingData;
        }

        public override bool HasMember(UUID memberId)
        {
            return Members.Select(e => e.Id).Contains(memberId);
        }

        [Obsolete]
        protected override bool KickMember(Member member, bool sendPacket = true)
        {
            throw new NotSupportedException();
        }

        [Obsolete]
        public override Member? GetMember(UUID uuid)
        {
            throw new NotSupportedException();
        }

        public override int GetMemberCount()
        {
            return Members.Count;
        }

        public override IEnumerable<UUID> GetMembersUUID()
        {
            return Members.Select(e => e.Id);
        }

        [Obsolete]
        public override void BuildMembers(List<RoomSyncPacket.SyncMember> members)
        {
            throw new NotSupportedException();
        }
    }
}
