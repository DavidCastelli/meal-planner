import { Routes } from '@angular/router';
import { RegisterComponent } from './core/auth/pages/register/register.component';
import { LoginComponent } from './core/auth/pages/login/login.component';
import { PageNotFoundComponent } from './core/errors/pages/page-not-found/page-not-found.component';
import { LandingComponent } from './core/layout/pages/landing/landing.component';
import { HomeComponent } from './core/layout/pages/home/home.component';
import {authGuard} from "./core/auth/auth.guard";
import {loginGuard} from "./core/auth/login.guard";

export const routes: Routes = [
  {
    path: 'landing',
    component: LandingComponent,
    title: 'Landing',
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
    canActivate: [loginGuard],
  },
  {
    path: 'home',
    component: HomeComponent,
    title: 'Home',
    canActivate: [authGuard],
  },
  {
    path: '',
    redirectTo: '/landing',
    pathMatch: 'full',
  },
  {
    path: '**',
    component: PageNotFoundComponent,
    title: 'PageNotFound',
  },
];
