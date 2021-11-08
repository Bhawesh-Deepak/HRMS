using HRMS.Core.Entities.Master;
using HRMS.Core.Entities.Payroll;
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
            //connectionString for the DataBase Migration
            //server=122.160.49.247,1433; Database=Payroll_Devlopment;User Id=SqlAdmin;Password=SqlAdmin@123
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
        public virtual DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }
        public virtual DbSet<EmployeeNonCTC> EmployeeNonCTCs { get; set; }
        public virtual DbSet<EmployeeDetail> EmployeeDetails { get; set; }
        public virtual DbSet<EmployeeSalaryDetail> EmployeeSalaryDetails { get; set; }

        #endregion
    }
}
