using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveManagementSystem.Models;

public class LeaveApplication
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Employee is required.")]

    [Display(Name = "Employee Id")]
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    
    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Display(Name = "Leave Type")]
    public string LeaveType { get; set; }

    [Display(Name = "Applied On")]
    [DataType(DataType.Date)]
    public DateTime AppliedOn { get; set; }
}