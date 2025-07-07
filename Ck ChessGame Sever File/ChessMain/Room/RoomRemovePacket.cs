using Runetide.Buffer;
using Runetide.Packet;

namespace EndoAshu.Chess.Room
{
    /// <summary>
    /// 룸 삭제 패킷
    /// Request는 Client -> Server 고정
    /// Response는 Server -> Client 고정
    /// </summary>
    public class RoomRemovePacket
    {
        public enum RemoveStatus
        {
            //성공
            SUCCESS,
            //이미 삭제됨
            REMOVED,
            //권한 없음
            NOT_HOST,
            //실패
            FAILED,
            TIMEOUT,
        }

        public abstract class Request : BasePacket
        {
            public static readonly int PacketId = 0x2000 | 0xa;

            protected Request()
            {

            }

            protected Request(RunetideBuffer buf) : base(buf)
            {

            }

            public override void Write(RunetideBuffer buf)
            {

            }
        }

        public abstract class Response : BasePacket
        {
            public static readonly int PacketId = 0x2000 | 0xb;

            public RemoveStatus Status { get; }

            protected Response(RemoveStatus status)
            {
                Status = status;
            }

            protected Response(RunetideBuffer buf) : base(buf)
            {
                Status = buf.ReadEnum<RemoveStatus>();
            }

            public override void Write(RunetideBuffer buf)
            {
                buf.WriteEnum(Status);
            }
        }
    }
}
