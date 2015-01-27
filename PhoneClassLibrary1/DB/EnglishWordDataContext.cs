using System.Data.Linq;

using PhoneClassLibrary1.Models;

namespace PhoneClassLibrary1.DB
{
    public class EnglishWordDataContext : DataContext
    {
        public const string ConnectionString = @"Data Source=isostore:/englishword.sdf";

        public EnglishWordDataContext(string connectionString) : base(connectionString) { }

        public Table<EnglishWord> EnglishWords;

    }
}
