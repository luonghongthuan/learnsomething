using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace PhoneClassLibrary1.Models
{
    [Table]
    public class EnglishWord
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false)]
        public int Id { get; set; }
        [Column]
        public string Word { get; set; }
        [Column]
        public string Meaning { get; set; }
        [Column]
        [DefaultValue("false")]
        public bool IsLearn { get; set; }
    }   
}
