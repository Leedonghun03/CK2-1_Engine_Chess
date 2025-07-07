using Runetide.Attr;
using Runetide.Util;
using System.Text.Json.Serialization;

namespace EndoAshu.Chess.User
{
    public abstract class UserAccount
    {
        public class Data
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("pw")]
            public string Password { get; set; } = string.Empty;

            [JsonPropertyName("username")]
            public string Username { get; set; } = string.Empty;

            [JsonPropertyName("uuid")]

            [JsonConverter(typeof(UUID.Converter))]
            public UUID UniqueId { get; set; } = UUID.NULL;

            [JsonPropertyName("win")]
            public int Win { get; set; } = 0;

            [JsonPropertyName("lose")]
            public int Lose { get; set; } = 0;

            [JsonPropertyName("draw")]
            public int Draw { get; set; } = 0;

            public override string ToString()
            {
                return $"Data[UniqueId={UniqueId}, Id={Id}, Username={Username}, Win={Win}, Lose={Lose}, Draw={Draw}]";
            }
        }

        public static readonly AttributeKey<UserAccount> ACCOUNT_KEY = AttributeKey<UserAccount>.Get("user_account");

        internal protected Data data;

        public string Id => data.Id;
        public UUID UniqueId => data.UniqueId;
        public string Username
        {
            get => data.Username; set => data.Username = value;
        }

        public int Win
        {
            get => data.Win; set => data.Win = value;
        }

        public int Lose
        {
            get => data.Lose; set => data.Lose = value;
        }

        public int Draw
        {
            get => data.Draw; set => data.Draw = value;
        }

        public UserAccount(Data data)
        {
            this.data = data;
        }

        public override string ToString()
        {
            return $"{GetType().Name}[Data={data.ToString()}]";
        }
    }
}
