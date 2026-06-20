import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

export const API_BASE = 'http://localhost:8080/api';

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private http: HttpClient) { }

  // ── Dashboard ─────────────────────────────────────────────────
  getDashboard() { return this.http.get<any>(`${API_BASE}/Dashboard`); }

  // ── Auth ──────────────────────────────────────────────────────
  login(data: any) { return this.http.post<any>(`${API_BASE}/auth/login`, data); }
  register(data: any) { return this.http.post<any>(`${API_BASE}/auth/register`, data); }
  forgotPassword(email: string) { return this.http.post<any>(`${API_BASE}/auth/forgot-password`, { email }); }
  resetPassword(data: any) { return this.http.post<any>(`${API_BASE}/auth/reset-password`, data); }

  // ── Students ──────────────────────────────────────────────────
  getStudents(params?: any) { return this.http.get<any>(`${API_BASE}/students`, { params }); }
  getStudent(id: number) { return this.http.get<any>(`${API_BASE}/students/${id}`); }
  createStudent(data: any) { return this.http.post<any>(`${API_BASE}/students`, data); }
  updateStudent(id: number, data: any) { return this.http.put<any>(`${API_BASE}/students/${id}`, data); }
  deleteStudent(id: number) { return this.http.delete<any>(`${API_BASE}/students/${id}`); }

  // ── Staff ─────────────────────────────────────────────────────
  getStaff(params?: any) { return this.http.get<any>(`${API_BASE}/staff`, { params }); }
  getStaffMember(id: number) { return this.http.get<any>(`${API_BASE}/staff/${id}`); }
  createStaff(data: any) { return this.http.post<any>(`${API_BASE}/staff`, data); }
  updateStaff(id: number, data: any) { return this.http.put<any>(`${API_BASE}/staff/${id}`, data); }
  deleteStaff(id: number) { return this.http.delete<any>(`${API_BASE}/staff/${id}`); }

  // ── Courses ───────────────────────────────────────────────────
  getCourses() { return this.http.get<any>(`${API_BASE}/courses`); }
  getCourse(id: number) { return this.http.get<any>(`${API_BASE}/courses/${id}`); }
  createCourse(data: any) { return this.http.post<any>(`${API_BASE}/courses`, data); }
  updateCourse(id: number, data: any) { return this.http.put<any>(`${API_BASE}/courses/${id}`, data); }
  deleteCourse(id: number) { return this.http.delete<any>(`${API_BASE}/courses/${id}`); }

  // ── Attendance ────────────────────────────────────────────────
  getAttendance(courseId: number, date: string) { return this.http.get<any>(`${API_BASE}/attendance?courseId=${courseId}&date=${date}`); }
  saveAttendance(data: any) { return this.http.post<any>(`${API_BASE}/attendance/bulk`, data); }
  getMonthlyAttendance(studentId: number, month: number, year: number) {
    return this.http.get<any>(`${API_BASE}/attendance/monthly?studentId=${studentId}&month=${month}&year=${year}`);
  }
  exportAttendanceCsv(courseId: number, month: number, year: number) {
    return this.http.get(`${API_BASE}/attendance/export?courseId=${courseId}&month=${month}&year=${year}`,
      { responseType: 'blob' });
  }

  // ── Payments ──────────────────────────────────────────────────
  getPayments(params?: any) { return this.http.get<any>(`${API_BASE}/payments`, { params }); }
  createPayment(data: any) { return this.http.post<any>(`${API_BASE}/payments`, data); }
  updatePayment(id: number, data: any) { return this.http.put<any>(`${API_BASE}/payments/${id}`, data); }
  deletePayment(id: number) { return this.http.delete<any>(`${API_BASE}/payments/${id}`); }
  getPendingPayments() { return this.http.get<any>(`${API_BASE}/payments/pending`); }

  // ── Gallery ───────────────────────────────────────────────────
  getGallery(category?: string) {
    let params: any = {};

    if (category) {
      params.category = category;
    }

    return this.http.get<any>(`${API_BASE}/gallery`, { params });
  }
  createGalleryItem(data: FormData) { return this.http.post<any>(`${API_BASE}/gallery`, data); }
  updateGalleryItem(id: number, data: any) { return this.http.put<any>(`${API_BASE}/gallery/${id}`, data); }
  deleteGalleryItem(id: number) { return this.http.delete<any>(`${API_BASE}/gallery/${id}`); }

  // ── Events ────────────────────────────────────────────────────
  getEvents(year?: number) {
    const params = year ? { year } : {};
    return this.http.get<any>(`${API_BASE}/events`, {});
  }
  createEvent(data: any) { return this.http.post<any>(`${API_BASE}/events`, data); }
  updateEvent(id: number, data: any) { return this.http.put<any>(`${API_BASE}/events/${id}`, data); }
  deleteEvent(id: number) { return this.http.delete<any>(`${API_BASE}/events/${id}`); }

  // ── Settings ──────────────────────────────────────────────────
  getSettings() { return this.http.get<any>(`${API_BASE}/settings`); }
  updateSettings(data: any) { return this.http.put<any>(`${API_BASE}/settings`, data); }

  // ── Public ────────────────────────────────────────────────────
  getPublicStats() { return this.http.get<any>(`${API_BASE}/public/stats`); }
  getPublicTestimonials() { return this.http.get<any>(`${API_BASE}/public/testimonials`); }
  getPublicEvents() { return this.http.get<any>(`${API_BASE}/public/events`); }
  getPublicCourses() { return this.http.get<any>(`${API_BASE}/public/courses`); }
  getPublicGallery() { return this.http.get<any>(`${API_BASE}/public/gallery`); }
  submitContact(data: any) { return this.http.post<any>(`${API_BASE}/public/contact`, data); }
  submitEnquiry(data: any) { return this.http.post<any>(`${API_BASE}/public/enquiry`, data); }

  // ── Batches ───────────────────────────────────────────────────
  getBatches(params?: any) { return this.http.get<any>(`${API_BASE}/batches`, { params }); }
  getBatch(id: number) { return this.http.get<any>(`${API_BASE}/batches/${id}`); }
  createBatch(data: any) { return this.http.post<any>(`${API_BASE}/batches`, data); }
  updateBatch(id: number, data: any) { return this.http.put<any>(`${API_BASE}/batches/${id}`, data); }
  deleteBatch(id: number) { return this.http.delete<any>(`${API_BASE}/batches/${id}`); }

  // ── Fee Plans ─────────────────────────────────────────────────
  getFeePlans() { return this.http.get<any>(`${API_BASE}/feeplans`); }
  getFeePlan(id: number) { return this.http.get<any>(`${API_BASE}/feeplans/${id}`); }
  createFeePlan(data: any) { return this.http.post<any>(`${API_BASE}/feeplans`, data); }
  updateFeePlan(id: number, data: any) { return this.http.put<any>(`${API_BASE}/feeplans/${id}`, data); }
  deleteFeePlan(id: number) { return this.http.delete<any>(`${API_BASE}/feeplans/${id}`); }

  // ── Testimonials (Admin) ───────────────────────────────────────
  getAdminTestimonials() { return this.http.get<any>(`${API_BASE}/testimonials`); }
  getAdminTestimonial(id: number) { return this.http.get<any>(`${API_BASE}/testimonials/${id}`); }
  createTestimonial(data: any) { return this.http.post<any>(`${API_BASE}/testimonials`, data); }
  updateTestimonial(id: number, data: any) { return this.http.put<any>(`${API_BASE}/testimonials/${id}`, data); }
  deleteTestimonial(id: number) { return this.http.delete<any>(`${API_BASE}/testimonials/${id}`); }

  // ── Contacts (Admin) ───────────────────────────────────────────
  getContacts() { return this.http.get<any>(`${API_BASE}/contacts`); }
  deleteContact(id: number) { return this.http.delete<any>(`${API_BASE}/contacts/${id}`); }

  // ── Enquiries (Admin) ──────────────────────────────────────────
  getEnquiries() { return this.http.get<any>(`${API_BASE}/enquiries`); }
  updateEnquiryStatus(id: number, status: string) { return this.http.put<any>(`${API_BASE}/enquiries/${id}/status`, JSON.stringify(status), { headers: { 'Content-Type': 'application/json' } }); }
  deleteEnquiry(id: number) { return this.http.delete<any>(`${API_BASE}/enquiries/${id}`); }

  // ── Notifications ──────────────────────────────────────────────
  getMyNotifications() { return this.http.get<any>(`${API_BASE}/notifications`); }
  markNotificationAsRead(id: number) { return this.http.put<any>(`${API_BASE}/notifications/${id}/read`, {}); }
  markAllNotificationsAsRead() { return this.http.put<any>(`${API_BASE}/notifications/read-all`, {}); }
}
