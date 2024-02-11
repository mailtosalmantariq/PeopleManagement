using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UKParliament.CodeTest.Data.DataContext;
using UKParliament.CodeTest.Services.People;
using UKParliament.CodeTest.Data.Models;

public class PersonServiceTests
{
    private readonly Mock<PersonManagerContext> _mockContext;
    private readonly PersonService _personService;

    public PersonServiceTests()
    {
        _mockContext = new Mock<PersonManagerContext>();
        _personService = new PersonService(_mockContext.Object);
    }

    [Fact]
    public async Task CreatePerson_Success_ReturnsTrue()
    {
        // Arrange
        var person = new Person
        {
            FirstName = "Salmaan",
            LastName = "Tariq",
            DOB = new DateTime(1990, 1, 1),
            DepartmentId = 1
        };

        // Act
        _mockContext.Setup(x => x.AddAsync(It.IsAny<Person>(), default)).Verifiable();
        _mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _personService.CreatePerson(person);

        // Assert
        Assert.True(result);
        _mockContext.Verify();
    }


    [Fact]
    public async Task DeletePerson_Success_ReturnsTrue()
    {
        // Arrange
        var person = new Person();

        // Act
        _mockContext.Setup(x => x.Remove(person)).Verifiable();
        _mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _personService.DeletePerson(person);

        // Assert
        Assert.True(result);
        _mockContext.Verify();
    }

    [Fact]
    public async Task GetPerson_Exists_ReturnsPerson()
    {
        // Arrange
        var personId = 1;
        var people = new List<Person> { new Person { Id = personId } }.AsQueryable();
        var mockDbSet = new Mock<DbSet<Person>>();
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(people.Provider);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(people.Expression);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(people.ElementType);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(people.GetEnumerator());

        _mockContext.Setup(x => x.People).Returns(mockDbSet.Object);

        // Act
        var result = await _personService.GetPerson(personId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(personId, result.Id);
    }
}
