using Runetide.Buffer;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.User
{
    public abstract class LoginPacket : BasePacket
    {
        public enum LoginStatus
        {
            SUCCESS,
            ALREADY_LOGINED,
            FAILED,
            PROTOCOL_MISMATCH,
            TIMEOUT
        }

        public abstract class Response : UserAccountSyncPacket
        {
            public static new readonly int PacketId = 0x1000 | 0x2;

            public LoginStatus Result { get; }

            public Response(LoginStatus status, UserAccount? account) : base(account)
            {
                this.Result = status;
            }

            public Response(RunetideBuffer buf) : base(buf)
            {
                this.Result = buf.ReadEnum<LoginStatus>();
            }

            public override void Write(RunetideBuffer buf)
            {
                base.Write(buf);
                buf.WriteEnum(this.Result);
            }
        }

        public static readonly int PacketId = 0x1000 | 0x1;
        protected readonly string Id;
        protected readonly string Password;
        protected readonly long clientProtocolVer;

        public LoginPacket(string id, string password, long clientProtocolVer)
        {
            Id = id;
            Password = password;
            this.clientProtocolVer = clientProtocolVer;
        }

        public LoginPacket(RunetideBuffer buf) : base(buf)
        {
            Id = buf.ReadString();
            Password = buf.ReadString();
            clientProtocolVer = buf.ReadInt64();
        }

        public override void Write(RunetideBuffer buf)
        {
            buf.WriteString(Id);
            buf.WriteString(Password);
            buf.WriteInt64(clientProtocolVer);
        }
    }
}
