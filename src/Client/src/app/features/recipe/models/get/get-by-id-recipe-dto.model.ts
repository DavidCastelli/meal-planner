import {GetByIdRecipeDetailsDto} from "./get-by-id-recipe-details-dto.model";
import {GetByIdRecipeNutritionDto} from "./get-by-id-recipe-nutrition-dto.model";
import {GetByIdRecipeDirectionDto} from "./get-by-id-recipe-direction-dto.model";
import {GetByIdRecipeTipDto} from "./get-by-id-recipe-tip-dto.model";
import {GetByIdRecipeMealDto} from "./get-by-id-recipe-meal-dto.model";
import {GetByIdRecipeSubIngredientDto} from "./get-by-id-recipe-subingredient-dto.model";

export interface GetByIdRecipeDto {
  Id: number;
  Title: string;
  Description?: string;
  Details: GetByIdRecipeDetailsDto;
  Nutrition: GetByIdRecipeNutritionDto;
  Directions: GetByIdRecipeDirectionDto[];
  Tips: GetByIdRecipeTipDto[];
  Meals: GetByIdRecipeMealDto[];
  SubIngredients: GetByIdRecipeSubIngredientDto[];
  ApplicationUserId: number;
}
