export class PersonViewModel {
  id?: number | null;
  firstName: string;
  lastName: string;
  dob: Date | string; // Change the type to Date | string
  department: string;

  constructor(id: number | null, firstName: string, lastName: string, dob: Date | string, department: string) {
    this.id = id;
    this.firstName = firstName;
    this.lastName = lastName;
    this.dob = dob;
    this.department = department;
  }
}
