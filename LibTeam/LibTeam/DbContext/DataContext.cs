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

        // DbSet cho các bảng
        public DbSet<NhaXuatBan> NhaXuatBans { get; set; }
        public DbSet<TuaSach> TuaSaches { get; set; }
        public DbSet<ThuVien> ThuViens { get; set; }
        public DbSet<SachTacGia> SachTacGias { get; set; }
        public DbSet<DocGia> DocGias { get; set; }
        public DbSet<CuonSach> CuonSaches { get; set; }
        public DbSet<MuonSach> MuonSaches { get; set; }
        public DbSet<TaiKhoanModel> TaiKhoanModels { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Gọi cấu hình mặc định của Identity trước
            base.OnModelCreating(modelBuilder);

            // --- Cấu hình cho NhaXuatBan ---
            modelBuilder.Entity<NhaXuatBan>(entity =>
            {
                entity.HasKey(nxb => nxb.NhaXuatBanID);
                entity.Property(nxb => nxb.TenNXB)
                      .IsRequired()
                      .HasMaxLength(150);
                entity.HasMany(nxb => nxb.TuaSaches)
                      .WithOne(ts => ts.NhaXuatBan)
                      .HasForeignKey(ts => ts.NhaXuatBanID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --- Cấu hình cho TuaSach ---
            modelBuilder.Entity<TuaSach>(entity =>
            {
                entity.HasKey(ts => ts.TuaSachID);
                entity.Property(ts => ts.TenTuaSach)
                      .IsRequired()
                      .HasMaxLength(150);
                entity.HasOne(ts => ts.NhaXuatBan)
                      .WithMany(nxb => nxb.TuaSaches)
                      .HasForeignKey(ts => ts.NhaXuatBanID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --- Cấu hình cho ThuVien ---
            modelBuilder.Entity<ThuVien>(entity =>
            {
                entity.HasKey(tv => tv.MaThuVien);
                entity.Property(tv => tv.TenThuVien)
                      .IsRequired()
                      .HasMaxLength(150);
            });

            // --- Cấu hình cho SachTacGia ---
            modelBuilder.Entity<SachTacGia>(entity =>
            {
                entity.HasKey(stg => stg.SachTacGiaID);
                entity.Property(stg => stg.TenTacGia)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.HasOne(stg => stg.TuaSach)
                      .WithMany(ts => ts.SachTacGias)
                      .HasForeignKey(stg => stg.TuaSachID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --- Cấu hình cho DocGia ---
            modelBuilder.Entity<DocGia>(entity =>
            {
                entity.HasKey(dg => dg.SoTheDG);
                entity.Property(dg => dg.TenDG)
                      .IsRequired()
                      .HasMaxLength(100);
            });

            // --- Cấu hình cho CuonSach ---
            modelBuilder.Entity<CuonSach>(entity =>
            {
                entity.HasKey(cs => cs.CuonSachID);
                entity.HasOne(cs => cs.TuaSach)
                      .WithMany(ts => ts.CuonSaches)
                      .HasForeignKey(cs => cs.TuaSachID)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(cs => cs.ThuVien)
                      .WithMany(tv => tv.CuonSaches)
                      .HasForeignKey(cs => cs.ThuVienID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- Cấu hình cho MuonSach ---
            modelBuilder.Entity<MuonSach>(entity =>
            {
                entity.HasKey(ms => ms.MuonSachID);
                entity.HasOne(ms => ms.TuaSach)
                      .WithMany(ts => ts.MuonSaches)
                      .HasForeignKey(ms => ms.TuaSachID)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(ms => ms.ThuVien)
                      .WithMany(tv => tv.MuonSaches)
                      .HasForeignKey(ms => ms.ThuVienID)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(ms => ms.DocGia)
                      .WithMany() 
                      .HasForeignKey(ms => ms.SoTheDG)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            // --- Cấu hình cho TaiKhoanModel ---
            modelBuilder.Entity<TaiKhoanModel>(entity =>
            {
                entity.HasKey(tk => tk.Id);
                entity.Property(tk => tk.UserName)
                      .IsRequired()
                      .HasMaxLength(150);
                entity.Property(tk => tk.Email)
                      .IsRequired()
                      .HasMaxLength(150);
                entity.Property(tk => tk.Password)
                      .IsRequired();
            });
        }
    }
}
