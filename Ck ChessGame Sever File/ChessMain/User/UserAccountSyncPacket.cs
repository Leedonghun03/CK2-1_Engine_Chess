using Runetide.Buffer;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.User
{
    public abstract class UserAccountSyncPacket : BasePacket
    {
        public static readonly int PacketId = 0x1000 | 0x3;

        public UserAccount.Data? AccountData { get; }


        public UserAccountSyncPacket(UserAccount? account)
        {
            this.AccountData = account != null ? account.data : null;
        }

        public UserAccountSyncPacket(RunetideBuffer buf) : base(buf)
        {
            bool isNull = buf.ReadBool();
            if (isNull)
            {
                this.AccountData = new UserAccount.Data();
                this.AccountData.Id = buf.ReadString();
                this.AccountData.UniqueId = buf.ReadUUID();
                this.AccountData.Username = buf.ReadString();
                this.AccountData.Win = buf.ReadInt32();
                this.AccountData.Lose = buf.ReadInt32();
                this.AccountData.Draw = buf.ReadInt32();
            }
            else
                this.AccountData = null;
        }

        public override void Write(RunetideBuffer buf)
        {
            base.Write(buf);
            buf.WriteBool(this.AccountData != null);
            if (this.AccountData != null)
            {
                buf.WriteString(this.AccountData.Id);
                buf.WriteUUID(this.AccountData.UniqueId);
                buf.WriteString(this.AccountData.Username);
                buf.WriteInt32(this.AccountData.Win);
                buf.WriteInt32(this.AccountData.Lose);
                buf.WriteInt32(this.AccountData.Draw);
            }
        }
    }
}
