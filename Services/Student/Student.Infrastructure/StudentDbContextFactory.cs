using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Student.Infrastructure
{
    public class StudentDbContextFactory : IDesignTimeDbContextFactory<StudentDbContext>
    {
        public StudentDbContext CreateDbContext(string[] args)
        {
            // reads connection string from Student.Api/appsettings.json (relative path)
            var basePath = Directory.GetCurrentDirectory();
            // adjust path if your solution layout differs
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(basePath, "..", "Student.Api"))
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var conn = config.GetConnectionString("StudentDb")
                       ?? "Data Source=PC-OANHVU;Initial Catalog=Student;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

            var optionsBuilder = new DbContextOptionsBuilder<StudentDbContext>();
            optionsBuilder.UseSqlServer(conn);

            return new StudentDbContext(optionsBuilder.Options);
        }
    }
}
