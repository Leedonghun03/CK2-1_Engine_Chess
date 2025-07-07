using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using System;
using System.Collections.Generic;

namespace EndoAshu.Chess.Client.Room
{
    public sealed class ClientSideRoomSyncPacket : RoomSyncPacket
    {
        [Obsolete]
        public ClientSideRoomSyncPacket(AbstractRoom room) : base(room)
        {
            throw new System.NotSupportedException();
        }

        public ClientSideRoomSyncPacket(RunetideBuffer buf) : base(buf)
        {
        }

        public override List<SyncMember> GetMembers(AbstractRoom room)
        {
            throw new System.NotSupportedException();
        }

        public override void Handle(PacketContext<NetworkContext> ctx)
        {
            ctx.Get()?.GetAttribute(ChessClient.CHESS_CLIENT).IfPresent(client =>
            {
                ctx.MarkHandle();
                if (client.CurrentRoom == null)
                {
                    client.CurrentRoom = new ClientRoom();
                }
                if (PlayingData.IsPlaying)
                {
                    if (!(client.State is GameInState))
                    {
                        client.UpdateState(new GameInState(client));
                    }
                }
                else
                {
                    if (!(client.State is GameRoomState))
                    {
                        client.UpdateState(new GameRoomState(client));
                    }
                }
                client.CurrentRoom.HandleSync(this);
            });
        }
    }
}
