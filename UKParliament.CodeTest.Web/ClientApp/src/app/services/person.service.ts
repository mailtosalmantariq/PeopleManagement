import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PersonViewModel } from '../models/person-view-model';

@Injectable({
  providedIn: 'root'
})
//export class PersonService {
//  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

//  // Below is some sample code to help get you started calling the API
//  getById(id: number): Observable<PersonViewModel> {
//    return this.http.get<PersonViewModel>(this.baseUrl + `api/person/${id}`)
//  }
//}


export class PersonService {
  private baseUrl = 'https://localhost:7048/';

  constructor(private http: HttpClient) { }

  getAllPersons(): Observable<PersonViewModel[]> {
    return this.http.get<PersonViewModel[]>(`${this.baseUrl}api/person`);
  }

  getPersonById(id: number): Observable<PersonViewModel> {
    return this.http.get<PersonViewModel>(`${this.baseUrl}api/person/${id}`);
  }

  createPerson(person: PersonViewModel): Observable<any> {
    return this.http.post(`${this.baseUrl}api/person`, person);
  }

  updatePerson(person: PersonViewModel): Observable<any> {
    return this.http.put(`${this.baseUrl}api/person/${person.id}`, person);
  }

  deletePerson(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}api/person/${id}`);
  }
}
