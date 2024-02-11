using UKParliament.CodeTest.Data.Models;

namespace UKParliament.CodeTest.Services.Departments
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetDepartments();
        Task<Department?> GetDepartment(int departmentId);
    }
}
