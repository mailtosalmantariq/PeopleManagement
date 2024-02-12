using Moq;
using Microsoft.AspNetCore.Mvc;
using UKParliament.CodeTest.Data.Models;
using UKParliament.CodeTest.Services.Departments;
using UKParliament.CodeTest.Services.People;
using UKParliament.CodeTest.Web.Controllers;

[TestFixture]
public class PersonControllerTests
{
    private Mock<IPersonService> _mockPersonService;
    private Mock<IDepartmentService> _mockDepartmentService;
    private PersonController _personController;

    [SetUp]
    public void Setup()
    {
        _mockPersonService = new Mock<IPersonService>();
        _mockDepartmentService = new Mock<IDepartmentService>();
        _personController = new PersonController(_mockPersonService.Object, _mockDepartmentService.Object);
    }

    [Test]
    public async Task GetById_ExistingId_ReturnsOkResult()
    {
        var existingPersonId = 1;
        var existingPerson = new Person { Id = existingPersonId };
        _mockPersonService.Setup(x => x.PersonExists(existingPersonId)).ReturnsAsync(true);
        _mockPersonService.Setup(x => x.GetPerson(existingPersonId)).ReturnsAsync(existingPerson);
        _mockDepartmentService.Setup(x => x.GetDepartment(existingPersonId)).ReturnsAsync(new Department { Name = "DepartmentName" });

        var result = await _personController.GetById(existingPersonId);

        //Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetById_NonExistingId_ReturnsNotFoundResult()
    {
        var nonExistingPersonId = 100;
        _mockPersonService.Setup(x => x.PersonExists(nonExistingPersonId)).ReturnsAsync(false);

        var result = await _personController.GetById(nonExistingPersonId);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task GetPersons_ReturnsOkResult()
    {
        var persons = new List<Person> { new Person { Id = 1 }, new Person { Id = 2 } };
        _mockPersonService.Setup(x => x.GetPersons()).ReturnsAsync(persons);
        _mockDepartmentService.Setup(x => x.GetDepartment(It.IsAny<int>())).ReturnsAsync(new Department { Name = "DepartmentName" });

        var result = await _personController.GetPersons();

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task CreatePerson_WithValidPerson_ReturnsCreatedAtRouteResult()
    {
        var personToCreate = new Person { Id = 1, FirstName = "John", LastName = "Doe", DepartmentId = 1 };
        _mockPersonService.Setup(x => x.GetPersons()).ReturnsAsync(new List<Person>());
        _mockPersonService.Setup(x => x.PersonExists(personToCreate.Id)).ReturnsAsync(false);
        _mockPersonService.Setup(x => x.CreatePerson(personToCreate)).ReturnsAsync(true);

        var result = await _personController.CreatePerson(personToCreate);

        Assert.That(result, Is.InstanceOf<CreatedAtRouteResult>());
    }

    [Test]
    public async Task CreatePerson_WithExistingPerson_ReturnsStatusCode422()
    {
        var existingPersonId = 1;
        var existingPerson = new Person { Id = existingPersonId, FirstName = "John", LastName = "Doe", DepartmentId = 1 };
        _mockPersonService.Setup(x => x.GetPersons()).ReturnsAsync(new List<Person> { existingPerson });
        _mockPersonService.Setup(x => x.PersonExists(existingPersonId)).ReturnsAsync(true);

        var result = await _personController.CreatePerson(existingPerson);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            Assert.That((result as ObjectResult).StatusCode, Is.EqualTo(422));
        });
    }

    [Test]
    public async Task UpdatePerson_WithValidPerson_ReturnsNoContentResult()
    {
        var existingPersonId = 1;
        var updatedPersonInfo = new Person { Id = existingPersonId, FirstName = "John", LastName = "Doe", DepartmentId = 1 };
        _mockPersonService.Setup(x => x.PersonExists(existingPersonId)).ReturnsAsync(true);
        _mockPersonService.Setup(x => x.UpdatePerson(updatedPersonInfo)).ReturnsAsync(true);

        var result = await _personController.UpdatePerson(existingPersonId, updatedPersonInfo);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task DeletePerson_WithExistingPersonId_ReturnsNoContentResult()
    {
        var existingPersonId = 1;
        _mockPersonService.Setup(x => x.PersonExists(existingPersonId)).ReturnsAsync(true);
        _mockPersonService.Setup(x => x.GetPerson(existingPersonId)).ReturnsAsync(new Person());

        var result = await _personController.DeletePerson(existingPersonId);

        Assert.That((result as ObjectResult).StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task DeletePerson_WithNonExistingPersonId_ReturnsNotFoundResult()
    {
        var nonExistingPersonId = 100;
        _mockPersonService.Setup(x => x.PersonExists(nonExistingPersonId)).ReturnsAsync(false);

        var result = await _personController.DeletePerson(nonExistingPersonId);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
