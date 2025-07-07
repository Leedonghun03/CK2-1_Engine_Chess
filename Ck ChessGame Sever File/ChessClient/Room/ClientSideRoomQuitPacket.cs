using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Client.Room
{
    public sealed class ClientSideRoomQuitPacket
    {
        public sealed class Request : RoomQuitPacket.Request
        {
            public Request(RunetideBuffer buf) : base(buf)
            {

            }

            public Request(UUID roomId, UUID targetId) : base(roomId, targetId)
            {

            }

            public override void Handle(PacketContext<NetworkContext> ctx)
            {
                throw new System.NotSupportedException();
            }
        }

        public sealed class Response : RoomQuitPacket.Response
        {

            public Response(RunetideBuffer buf) : base(buf)
            {
            }

            public Response(UUID target, RoomQuitPacket.QuitStatus status) : base(target, status)
            {
            }

            public override void Handle(PacketContext<NetworkContext> ctx)
            {
                ctx.Get()?.GetAttribute(ChessClient.CHESS_CLIENT).IfPresent(client =>
                {
                    ctx.MarkHandle();
                    (client.State as GameRoomState)?.InternalOnQuitRoom(TargetId, Status);
                });
            }
        }
    }
}
