
import { Component, OnInit } from '@angular/core';
import { PersonViewModel } from '../../models/person-view-model';
import { PersonService } from '../../services/person.service';

@Component({
  selector: 'app-person-list',
  templateUrl: './person-list.component.html',
  /*styleUrls: ['./person-list.component.css']*/
})
export class PersonListComponent implements OnInit {
  persons: PersonViewModel[] = [];
  selectedPerson: PersonViewModel | null = null;

  constructor(private personService: PersonService) { }

  ngOnInit(): void {
    this.loadPersons();
  }

  loadPersons(): void {
    this.personService.getAllPersons().subscribe(persons => {
      this.persons = persons;
    });
  }

  selectPerson(person: PersonViewModel): void {
    this.selectedPerson = person;
  }

  clearSelection(): void {
    this.selectedPerson = null;
  }

  onPersonCreated(person: PersonViewModel): void {
    this.persons.push(person); // Add the newly created person to the list
  }
}

