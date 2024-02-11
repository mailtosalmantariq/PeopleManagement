using System.ComponentModel.DataAnnotations;

namespace UKParliament.CodeTest.Data.Models;

public class Department
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Department name is required.")]
    [StringLength(100, ErrorMessage = "Department name must not exceed 100 characters.")]
    public string Name { get; set; }
}
