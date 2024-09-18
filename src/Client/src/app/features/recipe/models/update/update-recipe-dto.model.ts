import {UpdateRecipeDetailsDto} from "./update-recipe-details-dto.model";
import {UpdateRecipeNutritionDto} from "./update-recipe-nutrition-dto.model";
import {UpdateRecipeDirectionDto} from "./update-recipe-direction-dto.model";
import {UpdateRecipeTipDto} from "./update-recipe-tip-dto.model";
import {UpdateRecipeSubIngredientDto} from "./update-recipe-subingredient-dto.model";

export interface UpdateRecipeDto {
  Id: number;
  Title: string;
  Description?: string;
  Details: UpdateRecipeDetailsDto;
  Nutrition: UpdateRecipeNutritionDto;
  Directions: UpdateRecipeDirectionDto[];
  Tips: UpdateRecipeTipDto[];
  SubIngredients: UpdateRecipeSubIngredientDto[];
}
