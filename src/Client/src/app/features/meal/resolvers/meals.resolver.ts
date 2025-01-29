import { ResolveFn } from '@angular/router';
import { GetMealsDto } from '../models/get/get-meals-dto.model';
import { MealService } from '../meal.service';
import { inject } from '@angular/core';
import { map } from 'rxjs';

export const mealsResolver: ResolveFn<GetMealsDto[]> = () => {
  return inject(MealService)
    .getMeals()
    .pipe(
      map((meals: GetMealsDto[]) => {
        return meals.sort((mealA, mealB) => {
          return mealA.title.localeCompare(mealB.title);
        });
      }),
    );
};
