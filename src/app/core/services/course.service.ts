import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CourseService {

  private apiUrl = 'http://localhost:5164/api/Courses';

  constructor(private http: HttpClient) {}

  getCourses() {
    return this.http.get<any[]>(this.apiUrl);
  }

  createCourse(course: any) {
    return this.http.post(this.apiUrl, course);
  }
}