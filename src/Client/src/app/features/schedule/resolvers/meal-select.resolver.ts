import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';
import { map } from 'rxjs';
import { GetMealsDto } from '../models/get-meals-dto.model';
import { ScheduleService } from '../schedule.service';

export const mealSelectResolver: ResolveFn<GetMealsDto[]> = () => {
  return inject(ScheduleService)
    .getMeals()
    .pipe(
      map((meals: GetMealsDto[]) => {
        return meals.sort((mealA, mealB) => {
          return mealA.title.localeCompare(mealB.title);
        });
      }),
    );
};
