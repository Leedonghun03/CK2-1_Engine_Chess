using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Client.Room
{
    public sealed class ClientSideRoomChangePlayerModePacket : RoomChangePlayerModePacket
    {
        public ClientSideRoomChangePlayerModePacket(UUID targetUUID, PlayerMode targetMode) : base(targetUUID, targetMode)
        {
        }

        public ClientSideRoomChangePlayerModePacket(RunetideBuffer buf) : base(buf)
        {
        }

        public override void Handle(PacketContext<NetworkContext> ctx)
        {
            throw new System.NotImplementedException();
        }
    }
}
