using Microsoft.EntityFrameworkCore;
using UKParliament.CodeTest.Data.DataContext;
using UKParliament.CodeTest.Data.Models;

namespace UKParliament.CodeTest.Services.People;

public class PersonService : IPersonService
{
    private readonly PersonManagerContext _personContext;

    public PersonService(PersonManagerContext personContext)
    {
        _personContext = personContext;
    }

    public async Task<bool> CreatePerson(Person person)
    {
        await _personContext.AddAsync(person);
        return await Save();
    }

    public async Task<bool> DeletePerson(Person person)
    {
        _personContext.Remove(person);
        return await Save();
    }

    public async Task<Person?> GetPerson(int personId)
    {
        return await _personContext.People.Where(p => p.Id == personId).FirstOrDefaultAsync();
    }
    public async Task<List<Person>> GetPersons()
    {
        return await _personContext.People.OrderBy(a => a.LastName).ToListAsync();
    }
    public async Task<bool> PersonExists(int personId)
    {
        return await _personContext.People.AnyAsync(p => p.Id == personId);
    }

    public async Task<bool> Save()
    {
        var saved = await _personContext.SaveChangesAsync();
        return saved >= 0 ? true : false;
    }

    public async Task<bool> UpdatePerson(Person person)
    {
        _personContext.Update(person);
        return await Save();
    }
}