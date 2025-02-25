import { GetMealByIdRecipeDetailsDto } from './get-meal-by-id-recipe-details-dto.model';
import { GetMealByIdRecipeNutritionDto } from './get-meal-by-id-recipe-nutrition-dto.model';

export interface GetMealByIdRecipeDto {
  id: number;
  title: string;
  description?: string;
  imageUrl?: string;
  details: GetMealByIdRecipeDetailsDto;
  nutrition: GetMealByIdRecipeNutritionDto;
}
