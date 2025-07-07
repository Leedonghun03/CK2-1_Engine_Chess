using System.Text.Json.Serialization;

namespace EndoAshu.Chess.Room
{
    public enum GameMode
    {
        ONE_VS_ONE,
        TWO_VS_TWO
    }

    public class RoomOptions
    {
        [JsonPropertyName("name")]
        [JsonRequired]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("mode")]
        [JsonRequired]
        public GameMode Mode { get; set; } = GameMode.ONE_VS_ONE;

        [JsonPropertyName("password")]
        public string? Password { get; set; } = null;

        public bool CheckAllowed()
        {
            if (string.IsNullOrWhiteSpace(Name)) return false;
            if (Name.Length < 4 || Name.Length > 30) return false;
            if (Password != null && (Password.Length < 4 || Password.Length > 20)) return false;
            return true;
        }

        public override string ToString()
        {
            return $"RoomOptions[Name={Name}, Mode={Mode}, Password={Password}]";
        }
    }
}
