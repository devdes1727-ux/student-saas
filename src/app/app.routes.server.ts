import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  {
    path: 'admin/students/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'admin/staff/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'admin/courses/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'admin/batches/:id',
    renderMode: RenderMode.Server
  },
  {
    path: '**',
    renderMode: RenderMode.Prerender
  }
];
