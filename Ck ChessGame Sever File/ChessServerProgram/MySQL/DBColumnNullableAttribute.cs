namespace ChessServerProgram.MySQL
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DBColumnNullableAttribute : System.Attribute
    {
        public DBColumnNullableAttribute() { }
    }
}
