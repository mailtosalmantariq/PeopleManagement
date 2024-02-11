using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using UKParliament.CodeTest.Data.Models;
using UKParliament.CodeTest.Services.Departments;
using UKParliament.CodeTest.Services.People;
using UKParliament.CodeTest.Web.ViewModels;

namespace UKParliament.CodeTest.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonController : ControllerBase
{
    private readonly IPersonService _personService;
    private readonly IDepartmentService _departmentService;
    public PersonController(IPersonService personService, IDepartmentService departmentService)
    {
        _personService = personService;
        _departmentService = departmentService;
    }

    [Route("{id:int}")]
    [HttpGet]
    public async Task<ActionResult> GetById(int id)
    {
        try
        {
            if (!await _personService.PersonExists(id))
                return NotFound();

            var person = await _personService.GetPerson(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var department = await _departmentService.GetDepartment(id);

            var personViewModel = new PersonViewModel()
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                DepartmentId = person.DepartmentId,
                DepartmentName = department.Name
            };

            return Ok(personViewModel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPersons()
    {
        try
        {
            var persons = await _personService.GetPersons();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var personViewModel = new List<PersonViewModel>();

            foreach (var person in persons)
            {
                var department = await _departmentService.GetDepartment(person.Id);
                personViewModel.Add(new PersonViewModel
                {
                    Id = person.Id,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    DepartmentId = person.DepartmentId,
                    DepartmentName = department.Name
                });
            }

            return Ok(personViewModel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreatePerson([FromBody] Person personToCreate)
    {
        try
        {

            if (personToCreate == null)
                return BadRequest(ModelState);

            var persons = await _personService.GetPersons();
            var person = persons.Where(p => p.FirstName.Trim().ToUpper() == personToCreate.FirstName.Trim().ToUpper()
                && p.LastName.Trim().ToUpper() == personToCreate.LastName.Trim().ToUpper()).FirstOrDefault();

            if (person != null)
            {
                ModelState.AddModelError("", $"Person {personToCreate.FirstName} already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (!await _personService.CreatePerson(personToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving {personToCreate.FirstName}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetPersonById", new { personId = personToCreate.Id }, personToCreate);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }
    }

    [HttpPut("{personId}")]
    public async Task<IActionResult> UpdatePerson(int personId, [FromBody] Person updatedPersonInfo)
    {
        try
        {
            if (updatedPersonInfo == null)
                return BadRequest(ModelState);

            if (personId != updatedPersonInfo.Id)
                return BadRequest(ModelState);

            if (!await _personService.PersonExists(personId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _personService.UpdatePerson(updatedPersonInfo))
            {
                ModelState.AddModelError("", $"Something went wrong updating {updatedPersonInfo.FirstName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }
    }

    [HttpDelete("{personId}")]
    public async Task<IActionResult> DeletePerson(int personId)
    {
        try
        {


            if (!await _personService.PersonExists(personId))
                return NotFound();

            var personToDelete = await _personService.GetPerson(personId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _personService.DeletePerson(personToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {personToDelete.FirstName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }
    }

}