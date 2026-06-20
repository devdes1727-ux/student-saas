import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class StudentService {

    private apiUrl = 'http://localhost:8080/api/Students';

    constructor(private http: HttpClient) { }

    getStudents(): Observable<any[]> {
        return this.http.get<any[]>(this.apiUrl);
    }

    addStudent(student: any): Observable<any> {
        return this.http.post(this.apiUrl, student);
    }

    deleteStudent(id: number): Observable<any> {
        return this.http.delete(`${this.apiUrl}/${id}`);
    }
}