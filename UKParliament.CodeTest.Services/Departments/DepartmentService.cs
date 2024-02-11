using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UKParliament.CodeTest.Data.DataContext;
using UKParliament.CodeTest.Data.Models;

namespace UKParliament.CodeTest.Services.Departments
{
    public class DepartmentService : IDepartmentService
    {
        private readonly PersonManagerContext _personContext;
        public DepartmentService(PersonManagerContext personContext)
        {
            _personContext = personContext;
        }

        public async Task<Department?> GetDepartment(int departmentId)
        {
            return await _personContext.Departments.Where(d => d.Id == departmentId).FirstOrDefaultAsync();
        }

        public async Task<List<Department>> GetDepartments()
        {
            return await _personContext.Departments.OrderBy(d => d.Name).ToListAsync();
        }
    }
}
