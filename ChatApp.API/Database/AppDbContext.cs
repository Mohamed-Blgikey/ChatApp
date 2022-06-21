using ChatApp.API.Entites;
using ChatApp.API.Extend;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Database
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options){}
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Message>()
                .HasOne(m=>m.Sender)
                .WithMany(u=>u.MessageSent)
                .HasForeignKey(u=>u.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Reciver)
                .WithMany(u => u.MessageRecived)
                .HasForeignKey(u => u.ReciverId)
                .OnDelete(DeleteBehavior.Restrict);
            base.OnModelCreating(builder);
        }
        public DbSet<Message> Messages { get; set; }
    }
}
