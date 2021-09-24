namespace QHH.Data.Context
{
    using Microsoft.EntityFrameworkCore;
    using QHH.Data.Models;

    /// <summary>
    /// Database Context and Context Options Builder for QHH.
    /// </summary>
    public class QHHDbContext : DbContext
    {
        /// <summary>
        /// Gets or Sets the <see cref="DbSet{TEntity}"/> containing all servers.
        /// </summary>
        public DbSet<Server> Servers { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="DbSet{TEntity}"/> containing all Achievement Diaries.
        /// </summary>
        public DbSet<AchievementDiary> AchievementDiaries { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<AutoRole> AutoRoles { get; set; }
        public DbSet<FAQ> FAQs { get; set; }

        /// <summary>
        /// Configure & Create DB Connection.
        /// </summary>
        /// <param name="options">NA.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseMySql(
                "server=localhost;user=root;database=qhh_test;port=3306;Connect Timeout=5",
                ServerVersion.AutoDetect("server=localhost;user=root;database=qhh_test;port=3306;Connect Timeout=5"));
    }
}
