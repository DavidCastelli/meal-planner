import { GetRecipeByIdDetailsDto } from './get-recipe-by-id-details-dto.model';
import { GetRecipeByIdNutritionDto } from './get-recipe-by-id-nutrition-dto.model';
import { GetRecipeByIdDirectionDto } from './get-recipe-by-id-direction-dto.model';
import { GetRecipeByIdTipDto } from './get-recipe-by-id-tip-dto.model';
import { GetRecipeByIdMealDto } from './get-recipe-by-id-meal-dto.model';
import { GetRecipeByIdSubIngredientDto } from './get-recipe-by-id-subingredient-dto.model';
import { GetRecipeByIdImageDto } from './get-recipe-by-id-image-dto.model';

export interface GetRecipeByIdDto {
  id: number;
  title: string;
  image?: GetRecipeByIdImageDto;
  description?: string;
  details: GetRecipeByIdDetailsDto;
  nutrition: GetRecipeByIdNutritionDto;
  directions: GetRecipeByIdDirectionDto[];
  tips: GetRecipeByIdTipDto[];
  meals: GetRecipeByIdMealDto[];
  subIngredients: GetRecipeByIdSubIngredientDto[];
  applicationUserId: number;
}
