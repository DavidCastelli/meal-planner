import { Routes } from '@angular/router';
import { RegisterComponent } from './core/auth/pages/register/register.component';

export const routes: Routes = [
  {
    path: 'register',
    component: RegisterComponent,
    title: 'Registration Page',
  },
];
