using EndoAshu.Chess.User;
using System;

namespace EndoAshu.Chess.Server.User
{
    public class ServerUserAccount : UserAccount
    {
        public string Password
        {
            get => data.Password;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Password cannot be null or empty.");
                data.Password = value;
            }
        }

        public ServerUserAccount(Data data) : base(data)
        {
        }

        public virtual void Save(bool cacheOnly = false)
        {
            //TODO : 유저 정보가 수정되면 호출되므로, DB에 반영 로직 추가
        }
    }
}
