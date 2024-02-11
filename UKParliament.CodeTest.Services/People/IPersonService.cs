using UKParliament.CodeTest.Data.Models;

namespace UKParliament.CodeTest.Services.People;

public interface IPersonService
{
    Task<List<Person>> GetPersons();    //To get all Persons
    Task<Person?> GetPerson(int personId);            //To get the Person by personID
    Task<bool> PersonExists(int personId);            //To check whether the person exist by personId
    Task<bool> CreatePerson(Person person);           //To create Person and return true or fasle
    Task<bool> UpdatePerson(Person person);           //To update Person and return true or false
    Task<bool> DeletePerson(Person person);           //To Delete Person and return true or false
    Task<bool> Save();
}