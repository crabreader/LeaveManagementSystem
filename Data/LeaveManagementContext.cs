using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Data
{
    public class LeaveManagementContext : DbContext
    {
        public LeaveManagementContext (DbContextOptions<LeaveManagementContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employee { get; set; } = default!;
        public DbSet<LeaveApplication> LeaveApplication { get; set; } = default!;
    }
}
