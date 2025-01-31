using Microsoft.EntityFrameworkCore;

namespace TinasLabb03.Model
{
    class SaveDBContext : DbContext
    {
        public DbSet<QuestionPack> QuestionPack { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "mongodb://localhost:27017";
            var collection = "TinaLagesson";

            optionsBuilder.UseMongoDB(connectionString, collection);
        }
    }

}
