import { Routes } from '@angular/router';
import { RegisterComponent } from './core/auth/pages/register/register.component';
import { LoginComponent } from './core/auth/pages/login/login.component';
import {HomeComponent} from "./core/layout/pages/home/home.component";

export const routes: Routes = [
  {
    path: 'register',
    component: RegisterComponent,
    title: 'Registration Page',
  },
  {
    path: 'login',
    component: LoginComponent,
    title: 'Login Page',
  },
  {
    path: 'home',
    component: HomeComponent,
    title: 'Home',
  },
];
