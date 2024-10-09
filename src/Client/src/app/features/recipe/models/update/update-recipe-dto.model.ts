import { UpdateRecipeDetailsDto } from './update-recipe-details-dto.model';
import { UpdateRecipeNutritionDto } from './update-recipe-nutrition-dto.model';
import { UpdateRecipeDirectionDto } from './update-recipe-direction-dto.model';
import { UpdateRecipeTipDto } from './update-recipe-tip-dto.model';
import { UpdateRecipeSubIngredientDto } from './update-recipe-subingredient-dto.model';

export interface UpdateRecipeDto {
  id: number;
  title: string;
  description?: string;
  details: UpdateRecipeDetailsDto;
  nutrition: UpdateRecipeNutritionDto;
  directions: UpdateRecipeDirectionDto[];
  tips: UpdateRecipeTipDto[];
  subIngredients: UpdateRecipeSubIngredientDto[];
}
