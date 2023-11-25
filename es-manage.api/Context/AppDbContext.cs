// Import library yang dibutuhkan
using es_manage.api.Models;
using Microsoft.EntityFrameworkCore;

// Membuat namespace
namespace es_manage.api.Context {
    // Membuat class AppDbContext yang mewarisi DbContext
    public class AppDbContext : DbContext {
        // Membuat constructor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Membuat DbSet untuk Table UserMst
        public DbSet<UserMst> UserMst { get; set; }

        // Membuat DbSet untuk Table ItemDepartment
        public DbSet<ItemDepartmentModel> ItemDepartment { get; set; }

        // Membuat DbSet untuk Table Brand
        public DbSet<BrandModel> Brand { get; set; }
    }
}