using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using System.Collections.Generic;

namespace EndoAshu.Chess.Server.Room
{
    public sealed class ServerSideRoomSyncPacket : RoomSyncPacket
    {
        public ServerSideRoomSyncPacket(AbstractRoom room) : base(room)
        {
        }

        public ServerSideRoomSyncPacket(RunetideBuffer buf) : base(buf)
        {
        }

        public override List<SyncMember> GetMembers(AbstractRoom room)
        {
            List<SyncMember> member = new List<SyncMember>();
            if (room is ServerRoom serverRoom)
            {
                serverRoom.BuildMembers(member);
            }
            return member;
        }

        public override void Handle(PacketContext<NetworkContext> ctx)
        {
            throw new System.NotSupportedException();
        }
    }
}
