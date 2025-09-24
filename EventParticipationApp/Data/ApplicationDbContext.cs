using EventParticipationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParticipationApp.Data


{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Participation> Participations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // İlişkileri yapılandırma
            modelBuilder.Entity<Participation>()
                .HasOne(p => p.Event)
                .WithMany(e => e.Participations)
                .HasForeignKey(p => p.EventId);
        }
    }
}
