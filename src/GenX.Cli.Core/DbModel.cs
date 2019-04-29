using System.Collections.Generic;

namespace GenX.Cli.Core
{
    public class DbColumn
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public int? Size { get; set; }
        public bool IsNullable { get; set; }
        public List<ForeignKey> ForeignKeys { get; set; } = new List<ForeignKey>();
    }

    public class DbEntity
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public List<DbColumn> Columns { get; set; } = new List<DbColumn>();
        public List<PrimaryKey> PrimaryKeys { get; set; } = new List<PrimaryKey>();
        public List<ForeignKey> Relationships { get; set; } = new List<ForeignKey>();
    }

    public class ForeignKey
    {
        public string ForeignKeyEntity { get; set; }
        public string ForeignKeyColumn { get; set; }
        public string PrimaryKeyEntity { get; set; }
        public string PrimaryKeyColumn { get; set; }
    }

    public class PrimaryKey
    {
        public string Entity { get; set; }
        public string Name { get; set; }
    }

    public class DbModel
    {
        public List<DbEntity> Entities { get; set; } = new List<DbEntity>();
    }
}
