using HRMS.Core.Entities.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace HRMS.Core.Entities.Common
{
    /// <summary>
    /// This is the context class which is used to connect to database
    /// </summary>
    public class HRMSContext: DbContext
    {
        private readonly string _connectionString;
        public HRMSContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().Property("CreatedDate").HasDefaultValue(DateTime.Now);
        }

        #region Master

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<SalaryHeads> SalaryHeads { get; set; }
        public virtual DbSet<Designation> Designations { get; set; }

        #endregion
    }
}
