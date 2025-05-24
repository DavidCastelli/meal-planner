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
import { PrivateLayoutComponent } from './core/layout/private-layout/private-layout.component';
import { exitGuard } from './core/guards/exit.guard';
import { PublicLayoutComponent } from './core/layout/public-layout/public-layout.component';
import { LogoutComponent } from './core/pages/logout/logout.component';
import { EditRecipeComponent } from './features/recipe/pages/edit-recipe/edit-recipe.component';
import { EditMealComponent } from './features/meal/pages/edit-meal/edit-meal.component';
import { MealDetailsComponent } from './features/meal/pages/meal-details/meal-details.component';
import { RecipeDetailsComponent } from './features/recipe/pages/recipe-details/recipe-details.component';
import { recipeResolver } from './features/recipe/resolvers/recipe.resolver';
import { recipesResolver } from './features/recipe/resolvers/recipes.resolver';
import { mealResolver } from './features/meal/resolvers/meal.resolver';
import { mealsResolver } from './features/meal/resolvers/meals.resolver';
import { tagsResolver } from './features/meal/resolvers/tags.resolver';
import { ScheduleComponent } from './features/schedule/pages/schedule/schedule.component';
import { scheduleResolver } from './features/schedule/resolvers/schedule.resolver';
import { mealSelectResolver } from './features/schedule/resolvers/meal-select.resolver';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/landing',
    pathMatch: 'full',
  },
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      {
        path: '',
        children: [
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
            path: 'logout',
            component: LogoutComponent,
            title: 'Logout',
            canActivate: [authGuard],
          },
        ],
      },
    ],
  },
  {
    path: '',
    component: PrivateLayoutComponent,
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
            canDeactivate: [exitGuard],
            resolve: { tags: tagsResolver },
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
            resolve: { meals: mealsResolver },
          },
          {
            path: 'manage/recipes',
            component: RecipesComponent,
            title: 'Recipes',
            resolve: { recipes: recipesResolver },
          },
          {
            path: 'manage/meals/:id/details',
            component: MealDetailsComponent,
            title: 'Meal Details',
            resolve: { meal: mealResolver },
          },
          {
            path: 'manage/recipes/:id/details',
            component: RecipeDetailsComponent,
            title: 'Recipe Details',
            resolve: { recipe: recipeResolver },
          },
          {
            path: 'manage/meals/:id/edit',
            component: EditMealComponent,
            title: 'Edit Meal',
            canDeactivate: [exitGuard],
            resolve: { meal: mealResolver, tags: tagsResolver },
          },
          {
            path: 'manage/recipes/:id/edit',
            component: EditRecipeComponent,
            title: 'Edit Recipe',
            canDeactivate: [exitGuard],
            resolve: { recipe: recipeResolver },
          },
          {
            path: 'schedule',
            component: ScheduleComponent,
            title: 'Schedule',
            resolve: {
              schedule: scheduleResolver,
              meals: mealSelectResolver,
            },
          },
        ],
        canActivateChild: [authGuard],
      },
    ],
    canActivate: [authGuard],
  },
  {
    path: '404',
    component: PageNotFoundComponent,
    title: 'PageNotFound',
  },
  {
    path: '**',
    component: PageNotFoundComponent,
    title: 'PageNotFound',
  },
];
