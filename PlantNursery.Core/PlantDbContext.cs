using Microsoft.EntityFrameworkCore;

namespace PlantNursery.Core
{

    public class PlantDbContext : DbContext
    {
        public DbSet<PlantEntity> Plants { get; set; }
        public DbSet<CareRecordEntity> CareRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=plantnursery.db");
    }

    public class PlantEntity
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string CommonName { get; set; }
        public string Type { get; set; }
        public int WateringFrequencyDays { get; set; }
        public int Health { get; set; }
    }

    public class CareRecordEntity
    {
        public int Id { get; set; }
        public int PlantId { get; set; }
        public string Type { get; set; }
        public System.DateTime Date { get; set; }
        public string Note { get; set; }
    }
}
