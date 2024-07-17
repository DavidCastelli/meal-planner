import { Routes } from '@angular/router';
import { RegisterComponent } from './core/auth/pages/register/register.component';
import { LoginComponent } from './core/auth/pages/login/login.component';
import {PageNotFoundComponent} from "./core/errors/pages/page-not-found/page-not-found.component";
import { LandingComponent } from './core/layout/pages/landing/landing.component';

export const routes: Routes = [
  {
    path: 'landing',
    component: LandingComponent,
    title: 'Landing Page',
  },
  {
    path: 'register',
    component: RegisterComponent,
    title: 'Register',
  },
  {
    path: 'login',
    component: LoginComponent,
    title: 'Login',
  },
  {
    path: '',
    redirectTo: '/landing',
    pathMatch: 'full',
  },
  {
    path: '**',
    component: PageNotFoundComponent,
    title: 'PageNotFound'
  }
];
