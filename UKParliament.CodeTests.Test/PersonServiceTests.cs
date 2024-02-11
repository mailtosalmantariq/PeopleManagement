using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using UKParliament.CodeTest.Data.DataContext;
using UKParliament.CodeTest.Data.Models;
using UKParliament.CodeTest.Services.People;

public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    public TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerable<TResult>(expression);
    }

    public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        return Task.FromResult(Execute<TResult>(expression));
    }

    TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    { }

    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }

    public T Current => _inner.Current;

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return new ValueTask();
    }
}

[TestFixture]
public class PersonServiceTests
{
    private Mock<PersonManagerContext> _mockContext;
    private PersonService _personService;

    [SetUp]
    public void Setup()
    {
        _mockContext = new Mock<PersonManagerContext>();
        _personService = new PersonService(_mockContext.Object);
    }

    [Test]
    public async Task CreatePerson_Success_ReturnsTrue()
    {
        // Arrange
        var person = new Person();

        // Act
        _mockContext.Setup(x => x.AddAsync(person, default)).Verifiable();
        _mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _personService.CreatePerson(person);

        // Assert
        Assert.IsTrue(result);
        _mockContext.Verify();
    }

    [Test]
    public async Task DeletePerson_Success_ReturnsTrue()
    {
        // Arrange
        var person = new Person();

        // Act
        _mockContext.Setup(x => x.Remove(person)).Verifiable();
        _mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _personService.DeletePerson(person);

        // Assert
        Assert.IsTrue(result);
        _mockContext.Verify();
    }

    [Test]
    public async Task GetPerson_Exists_ReturnsPerson()
    {
        // Arrange
        var personId = 1;
        var person = new Person { Id = personId };
        var mockDbSet = new Mock<DbSet<Person>>();
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Person>(person.AsQueryable().Provider));
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(person.AsQueryable().Expression);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(person.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(() => person.AsQueryable().GetEnumerator());

        _mockContext.Setup(x => x.People).Returns(mockDbSet.Object);

        // Act
        var result = await _personService.GetPerson(personId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(personId, result.Id);
    }

    [Test]
    public async Task GetPersons_ReturnsListOfPersons()
    {
        // Arrange
        var peopleList = new List<Person>
        {
            new Person { Id = 1, FirstName = "John", LastName = "Doe" },
            new Person { Id = 2, FirstName = "Jane", LastName = "Smith" }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Person>>();
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Person>(peopleList.Provider));
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(peopleList.Expression);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(peopleList.ElementType);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(() => peopleList.GetEnumerator());

        _mockContext.Setup(x => x.People).Returns(mockDbSet.Object);

        // Act
        var result = await _personService.GetPersons();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

}
