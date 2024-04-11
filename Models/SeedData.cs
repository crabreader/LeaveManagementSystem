using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LeaveManagementSystem.Data;
using System;
using System.Linq;

namespace LeaveManagementSystem.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new LeaveManagementContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<LeaveManagementContext>>()))
        {
            // Look for any movies.
            if (context.Employee.Any() || context.LeaveApplication.Any())
            {
                return;   // DB has been seeded
            }
            context.Employee.AddRange(
                new Employee
                {
                    Name = "Jimmy"
                },
                new Employee
                {
                    Name = "Chris"
                },
                new Employee
                {
                    Name = "Alice"
                },
                new Employee
                {
                    Name = "Ben"
                }
            );
            context.SaveChanges();

            var employeeIds = context.Employee.Select(e => e.Id).ToList();
            string[] leaveTypes = ["Vacation", "Parental Leave", "Health Reasons"];

            var random = new Random();
            foreach (var employeeId in employeeIds)
            {
                var startDate = DateTime.Now.Date.AddDays(random.Next(1, 30));
                var endDate = startDate.AddDays(random.Next(1, 10));
                var appliedOn = DateTime.Now.Date.AddDays(-random.Next(1, 90));
                string leaveType = leaveTypes[random.Next(0, leaveTypes.Length)];

                context.LeaveApplication.Add(new LeaveApplication
                {
                    EmployeeId = employeeId,
                    StartDate = startDate,
                    EndDate = endDate,
                    LeaveType = leaveType,
                    AppliedOn = appliedOn
                });
            }
            context.SaveChanges();
        }
    }
}