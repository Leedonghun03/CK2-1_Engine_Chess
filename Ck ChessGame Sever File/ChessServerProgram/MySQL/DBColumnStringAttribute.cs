namespace ChessServerProgram.MySQL
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DBColumnStringAttribute : System.Attribute
    {
        public int Length { get; }

        public DBColumnStringAttribute(int length)
        {
            Length = length;
        }
    }
}
