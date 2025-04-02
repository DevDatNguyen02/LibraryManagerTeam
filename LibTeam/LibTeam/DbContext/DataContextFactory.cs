using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace LibTeam.DbContext
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DataContext>();
            // Gán connection string tĩnh hoặc đọc từ cấu hình
            builder.UseSqlServer("Server=DATNGUYENA0B5;Database=Lib;User Id=sa;Password=datboySP96;Encrypt=True;TrustServerCertificate=True;");

            return new DataContext(builder.Options);
        }
    }
}
