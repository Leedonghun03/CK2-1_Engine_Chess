namespace ChessServerProgram.MySQL
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DBColumnNameAttribute : System.Attribute
    {
        public string Name { get; }

        public DBColumnNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
