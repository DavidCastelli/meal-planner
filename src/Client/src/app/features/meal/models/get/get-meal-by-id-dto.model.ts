import { GetMealByIdImageDto } from './get-meal-by-id-image-dto.model';
import { GetMealByIdTagDto } from './get-meal-by-id-tag-dto.model';
import { GetMealByIdRecipeDto } from './get-meal-by-id-recipe-dto.model';
import { Schedule } from '../../../../shared/enums/schedule.enum';

export interface GetMealByIdDto {
  id: number;
  title: string;
  image?: GetMealByIdImageDto;
  schedule: Schedule;
  tags: GetMealByIdTagDto[];
  recipes: GetMealByIdRecipeDto[];
  applicationUserId: string;
}
