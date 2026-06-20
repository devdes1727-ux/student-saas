import { Routes } from '@angular/router';
import { PublicLayout } from './layout/public-layout/public-layout';
import { AdminLayout } from './layout/admin-layout/admin-layout';
import { TeacherLayout } from './layout/teacher-layout/teacher-layout';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  // Public pages
  {
    path: '',
    component: PublicLayout,
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'home', loadComponent: () => import('./pages/public/home/home').then(m => m.Home) },
      { path: 'about', loadComponent: () => import('./pages/public/about/about').then(m => m.About) },
      { path: 'courses', loadComponent: () => import('./pages/public/courses/courses').then(m => m.Courses) },
      { path: 'gallery', loadComponent: () => import('./pages/public/gallery/gallery').then(m => m.Gallery) },
      { path: 'events', loadComponent: () => import('./pages/public/events/events').then(m => m.Events) },
      { path: 'testimonials', loadComponent: () => import('./pages/public/testimonials/testimonials').then(m => m.Testimonials) },
      { path: 'contact', loadComponent: () => import('./pages/public/contact/contact').then(m => m.Contact) },
      { path: 'admission-enquiry', loadComponent: () => import('./pages/public/admission-enquiry/admission-enquiry').then(m => m.AdmissionEnquiry) }
    ]
  },

  // Auth pages
  { path: 'login', loadComponent: () => import('./pages/login/login').then(m => m.Login) },
  { path: 'register', loadComponent: () => import('./pages/register/register').then(m => m.Register) },
  { path: 'forgot-password', loadComponent: () => import('./pages/forgot-password/forgot-password').then(m => m.ForgotPassword) },
  { path: 'reset-password', loadComponent: () => import('./pages/reset-password/reset-password').then(m => m.ResetPassword) },
  { path: 'unauthorized', loadComponent: () => import('./pages/unauthorized/unauthorized').then(m => m.Unauthorized) },

  // Admin pages
  {
    path: 'admin',
    component: AdminLayout,
    canActivate: [roleGuard],
    data: { expectedRoles: ['Admin', 'InstituteAdmin', 'SuperAdmin'] },
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', loadComponent: () => import('./pages/admin/dashboard/dashboard').then(m => m.AdminDashboard) },
      { path: 'students', loadComponent: () => import('./pages/students/students').then(m => m.Students) },
      { path: 'students/:id', loadComponent: () => import('./pages/student-detail/student-detail').then(m => m.StudentDetail) },
      { path: 'staff', loadComponent: () => import('./pages/staff/staff').then(m => m.Staff) },
      { path: 'staff/:id', loadComponent: () => import('./pages/staff-detail/staff-detail').then(m => m.StaffDetail) },
      { path: 'courses', loadComponent: () => import('./pages/courses/courses').then(m => m.Courses) },
      { path: 'courses/:id', loadComponent: () => import('./pages/course-detail/course-detail').then(m => m.CourseDetail) },
      { path: 'batches', loadComponent: () => import('./pages/admin/batches/batches').then(m => m.AdminBatches) },
      { path: 'batches/:id', loadComponent: () => import('./pages/batch-detail/batch-detail').then(m => m.BatchDetail) },
      { path: 'attendance', loadComponent: () => import('./pages/attendance/attendance').then(m => m.Attendance) },
      { path: 'payments', loadComponent: () => import('./pages/payments/payments').then(m => m.Payments) },
      { path: 'events', loadComponent: () => import('./pages/admin/events/events').then(m => m.AdminEvents) },
      { path: 'gallery', loadComponent: () => import('./pages/gallery/gallery').then(m => m.Gallery) },
      { path: 'testimonials', loadComponent: () => import('./pages/admin/testimonials/testimonials').then(m => m.AdminTestimonials) },
      { path: 'reports', loadComponent: () => import('./pages/admin/reports/reports').then(m => m.AdminReports) },
      { path: 'settings', loadComponent: () => import('./pages/admin/settings/settings').then(m => m.AdminSettings) }
    ]
  },

  // Teacher pages
  {
    path: 'teacher',
    component: TeacherLayout,
    canActivate: [roleGuard],
    data: { expectedRoles: ['Teacher'] },
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', loadComponent: () => import('./pages/teacher/dashboard/dashboard').then(m => m.TeacherDashboard) },
      { path: 'attendance', loadComponent: () => import('./pages/attendance/attendance').then(m => m.Attendance) },
      { path: 'students', loadComponent: () => import('./pages/students/students').then(m => m.Students) }
    ]
  },

  { path: '**', redirectTo: '' }
];