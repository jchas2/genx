namespace GenX.Cli.Infrastructure.Oledb
{
    public class Constraints
    {
        public string PrimaryKeyTable { get; set; }
        public string PrimaryKeyColumn { get; set; }
        public string ForeignKeyTable { get; set; }
        public string ForeignKeyColumn { get; set; }
    }
}
