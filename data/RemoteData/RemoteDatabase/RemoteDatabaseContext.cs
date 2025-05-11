using data.RemoteData.RemoteDatabase.DAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace data.RemoteData.RemoteDataBase
{
    public class RemoteDatabaseContext : DbContext
    {
        public DbSet<GroupDAO> Groups { get; set; }
        public DbSet<UserDAO> Users { get; set; }
        public DbSet<PresenceDAO> Presences { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=45.67.56.214;Port=5421;Database=user7;Username=user7;Password=a8yLONBC;Include Error Detail=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupDAO>(entity =>
            {
                entity.ToTable("groups", "public"); 
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(g => g.Name).HasColumnName("Name");
            });

            modelBuilder.Entity<UserDAO>(entity =>
            {
                entity.ToTable("users", "public");
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.UserId).HasColumnName("userid");
                entity.Property(u => u.FIO).HasColumnName("fio");
                entity.Property(u => u.GroupId).HasColumnName("groupid");
            });

            modelBuilder.Entity<PresenceDAO>(entity =>
            {
                entity.ToTable("presence", "public");
                entity.HasKey(p => p.PresenceId);
                entity.Property(p => p.PresenceId).HasColumnName("presenceid");
                entity.Property(p => p.Date).HasColumnName("Date");
                entity.Property(p => p.LessonNumber).HasColumnName("lessonnumber");
                entity.Property(p => p.IsAttendance).HasColumnName("isattendance");
                entity.Property(p => p.UserId).HasColumnName("userid");
                entity.Property(p => p.GroupId).HasColumnName("groupid");
            });
        }
    }
}
