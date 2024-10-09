import { UpdateRecipeRequestRecipeDetails } from './update-recipe-request-recipe-details.model';
import { UpdateRecipeRequestRecipeNutrition } from './update-recipe-request-recipe-nutrition.model';
import { UpdateRecipeRequestDirection } from './update-recipe-request-direction.model';
import { UpdateRecipeRequestTip } from './update-recipe-request-tip.model';
import { UpdateRecipeRequestSubIngredient } from './update-recipe-request-subingredient.model';

export interface UpdateRecipeRequest {
  id: number;
  title: string;
  description?: string;
  details: UpdateRecipeRequestRecipeDetails;
  nutrition: UpdateRecipeRequestRecipeNutrition;
  directions: UpdateRecipeRequestDirection[];
  tips: UpdateRecipeRequestTip[];
  subIngredients: UpdateRecipeRequestSubIngredient[];
}
