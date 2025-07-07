namespace ChessServerProgram.MySQL
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DBColumnUniqueAttribute : System.Attribute
    {
        public DBColumnUniqueAttribute() { }
    }
}
