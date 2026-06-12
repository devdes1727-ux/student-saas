import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Dashboard } from './pages/dashboard/dashboard';
import { Students } from './pages/students/students';
import { Staff } from './pages/staff/staff';
import { Courses } from './pages/courses/courses';
import { CourseDetail } from './pages/course-detail/course-detail';
import { BatchDetail } from './pages/batch-detail/batch-detail';
import { Attendance } from './pages/attendance/attendance';
import { StudentDetail } from './pages/student-detail/student-detail';
import { StaffDetail } from './pages/staff-detail/staff-detail';
import { AdminDashboard } from './pages/admin-dashboard/admin-dashboard';
import { Clients } from './pages/clients/clients';
import { Subscriptions } from './pages/subscriptions/subscriptions';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'dashboard', component: Dashboard, canActivate: [authGuard] },
    { path: 'login', component: Login },
    { path: 'students', component: Students, canActivate: [authGuard] },
    { path: 'students/:id', component: StudentDetail },
    { path: 'staff', component: Staff, canActivate: [authGuard] },
    { path: 'staff/:id', component: StaffDetail },
    { path: 'courses', component: Courses, canActivate: [authGuard] },
    { path: 'courses/:id', component: CourseDetail },
    { path: 'batches/:id', component: BatchDetail },
    { path: 'attendance', component: Attendance, canActivate: [authGuard] },
    { path: 'admin', component: AdminDashboard },
    { path: 'admin/clients', component: Clients },
    { path: 'admin/subscriptions', component: Subscriptions },
];