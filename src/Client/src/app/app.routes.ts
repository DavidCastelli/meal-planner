import { Routes } from '@angular/router';
import { RegisterComponent } from './core/auth/pages/register/register.component';
import { LoginComponent } from './core/auth/pages/login/login.component';
import {PageNotFoundComponent} from "./core/errors/pages/page-not-found/page-not-found.component";

export const routes: Routes = [
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
    path: '**',
    component: PageNotFoundComponent,
    title: 'PageNotFound'
  }
];
