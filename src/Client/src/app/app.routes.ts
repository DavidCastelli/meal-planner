import { Routes } from '@angular/router';
import { RegisterComponent } from './core/auth/pages/register/register.component';
import { LoginComponent } from './core/auth/pages/login/login.component';
import { PageNotFoundComponent } from './core/errors/pages/page-not-found/page-not-found.component';
import { LandingComponent } from './core/pages/landing/landing.component';
import { HomeComponent } from './core/pages/home/home.component';
import { authGuard } from './core/auth/guards/auth.guard';
import { loginGuard } from './core/auth/guards/login.guard';
import { MealsComponent } from './features/meal/pages/meals/meals.component';
import { RecipesComponent } from './features/recipe/pages/recipes/recipes.component';
import { CreateMealComponent } from './features/meal/pages/create-meal/create-meal.component';
import { CreateRecipeComponent } from './features/recipe/pages/create-recipe/create-recipe.component';
import { NavigationComponent } from './core/layout/navigation/navigation.component';
import { exitGuard } from './shared/guards/exit.guard';

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
    path: '',
    component: NavigationComponent,
    children: [
      {
        path: '',
        children: [
          {
            path: 'home',
            component: HomeComponent,
            title: 'Home',
          },
          {
            path: 'manage/meals/create',
            component: CreateMealComponent,
            title: 'Create Meal',
          },
          {
            path: 'manage/recipes/create',
            component: CreateRecipeComponent,
            title: 'Create Recipe',
            canDeactivate: [exitGuard],
          },
          {
            path: 'manage/meals',
            component: MealsComponent,
            title: 'Meals',
          },
          {
            path: 'manage/recipes',
            component: RecipesComponent,
            title: 'Recipes',
          },
        ],
        canActivateChild: [authGuard],
      },
    ],
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
