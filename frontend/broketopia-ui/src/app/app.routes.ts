import { Routes } from '@angular/router';
import {MainLayoutComponent} from './layout/main-layout.component';
import {LandingComponent} from './pages/landing/landing.component';
import {LoginComponent} from './pages/login/login.component';
import {AuthGuard} from './core/auth.guard';

export const routes: Routes = [
  { path: '', component: LandingComponent },
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'dashboard', loadChildren: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'upload', loadChildren: () => import('./pages/upload/upload.component').then(m => m.UploadComponent) },
      { path: 'reports', loadChildren: () => import('./pages/reports/reports.component').then(m => m.ReportsComponent) },
      { path: 'stocks', loadChildren: () => import('./pages/stocks/stocks.component').then(m => m.StocksComponent) },
    ]
  },
  { path: '**', redirectTo: '' }
];
