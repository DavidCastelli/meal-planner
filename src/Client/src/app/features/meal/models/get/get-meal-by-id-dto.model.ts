import { GetMealByIdImageDto } from './get-meal-by-id-image-dto.model';
import { GetMealByIdTagDto } from './get-meal-by-id-tag-dto.model';
import { GetMealByIdRecipeDto } from './get-meal-by-id-recipe-dto.model';

export interface GetMealByIdDto {
  id: number;
  title: string;
  image?: GetMealByIdImageDto;
  tags: GetMealByIdTagDto[];
  recipes: GetMealByIdRecipeDto[];
  applicationUserId: string;
}
