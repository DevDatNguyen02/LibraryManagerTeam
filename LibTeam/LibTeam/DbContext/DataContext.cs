using LibTeam.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibTeam.DbContext
{
    public class DataContext : IdentityDbContext<AppUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<TuaSach> TuaSaches { get; set; }
        public DbSet<CuonSach> CuonSaches { get; set; }
        public DbSet<DocGia> DocGias { get; set; }
        public DbSet<MuonSach> MuonSaches { get; set; }
        public DbSet<TaiKhoanModel> TaiKhoanModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TuaSach
            modelBuilder.Entity<TuaSach>(entity =>
            {
                entity.HasKey(ts => ts.TuaSachID);
                entity.Property(ts => ts.TenTuaSach).IsRequired();
                entity.Property(ts => ts.MoTa).IsRequired();
                entity.HasMany(ts => ts.CuonSaches)
                      .WithOne(cs => cs.TuaSach)
                      .HasForeignKey(cs => cs.TuaSachID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // CuonSach
            modelBuilder.Entity<CuonSach>(entity =>
            {
                entity.HasKey(cs => cs.CuonSachID);
                entity.Property(cs => cs.TenSach).IsRequired();
                entity.Property(cs => cs.TenTacGia).IsRequired();
                entity.Property(cs => cs.TenNXB).IsRequired();
                entity.Property(cs => cs.SoLuong).IsRequired();
                entity.HasMany(cs => cs.MuonSaches)
                      .WithOne(ms => ms.CuonSach)
                      .HasForeignKey(ms => ms.CuonSachID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // DocGia
            modelBuilder.Entity<DocGia>(entity =>
            {
                entity.HasKey(dg => dg.SoTheDG);
                entity.Property(dg => dg.HoTen).IsRequired();
                entity.Property(dg => dg.DiaChi).IsRequired();
                entity.Property(dg => dg.SDT).IsRequired();
                entity.HasMany(dg => dg.MuonSachs)
                      .WithOne(ms => ms.DocGia)
                      .HasForeignKey(ms => ms.SoTheDG)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // MuonSach
            modelBuilder.Entity<MuonSach>(entity =>
            {
                entity.HasKey(ms => ms.MuonSachID);
                entity.Property(ms => ms.NgayMuon).IsRequired();
                // NgayTra is optional
            });

            // TaiKhoanModel (if needed)
            modelBuilder.Entity<TaiKhoanModel>(entity =>
            {
                entity.HasKey(tk => tk.Id);
                entity.Property(tk => tk.UserName).IsRequired();
                entity.Property(tk => tk.Email).IsRequired();
                entity.Property(tk => tk.Password).IsRequired();
            });
        }
    }
}
