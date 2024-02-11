// person-editor.component.ts
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { PersonViewModel } from '../../models/person-view-model';
import { PersonService } from '../../services/person.service';

@Component({
  selector: 'app-person-editor',
  templateUrl: './person-editor.component.html'
  /*styleUrls: ['./person-editor.component.css']*/
})
export class PersonEditorComponent {
  @Input() person: PersonViewModel | null = { firstName: '', lastName: '', dob: '', department: '' };
  @Output() personCreated = new EventEmitter<PersonViewModel>();

  constructor(private personService: PersonService) { }

  savePerson(): void {
    if (this.person) {
      if (this.person.id) {
        this.personService.updatePerson(this.person).subscribe(() => {
          // Handle success
        });
      } else {
        this.personService.createPerson(this.person).subscribe((createdPerson: PersonViewModel) => {
          this.personCreated.emit(createdPerson); // Emit event to notify parent component
        });
      }
    }
  }
}
