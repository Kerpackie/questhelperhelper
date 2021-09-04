using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace questhelperhelper.Database
{
    public partial class QHHEntities : DbContext
    {
        public virtual DbSet<DiaryDevelopmentStatus> DiaryDevelopmentStatus { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "qhh.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            optionsBuilder.UseSqlite(connection);
        }        
    }
}