import { ResolveFn } from '@angular/router';
import { GetMealByIdDto } from '../models/get/get-meal-by-id-dto.model';
import { inject } from '@angular/core';
import { MealService } from '../meal.service';
import { map } from 'rxjs';

export const mealResolver: ResolveFn<GetMealByIdDto> = (route) => {
  return inject(MealService)
    .getMeal(route.params['id'])
    .pipe(
      map((meal: GetMealByIdDto) => {
        meal.tags.sort((tagA, tagB) => {
          if (tagA.id > tagB.id) {
            return 1;
          } else if (tagA.id < tagB.id) {
            return -1;
          } else {
            return 0;
          }
        });

        meal.recipes.sort((recipeA, recipeB) => {
          if (recipeA.id > recipeB.id) {
            return 1;
          } else if (recipeA.id < recipeB.id) {
            return -1;
          } else {
            return 0;
          }
        });

        return meal;
      }),
    );
};
