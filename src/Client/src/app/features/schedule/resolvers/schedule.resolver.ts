import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';
import { ScheduleService } from '../schedule.service';
import { GetMealsDto } from '../models/get-meals-dto.model';

export const scheduleResolver: ResolveFn<GetMealsDto[]> = () => {
  return inject(ScheduleService).getMeals(true);
};
