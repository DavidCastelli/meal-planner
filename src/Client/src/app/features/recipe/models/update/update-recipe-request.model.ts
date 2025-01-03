import { UpdateRecipeRequestDetails } from './update-recipe-request-details.model';
import { UpdateRecipeRequestNutrition } from './update-recipe-request-nutrition.model';
import { UpdateRecipeRequestDirection } from './update-recipe-request-direction.model';
import { UpdateRecipeRequestTip } from './update-recipe-request-tip.model';
import { UpdateRecipeRequestSubIngredient } from './update-recipe-request-subingredient.model';

export interface UpdateRecipeRequest {
  id: number;
  title: string;
  description?: string;
  details: UpdateRecipeRequestDetails;
  nutrition: UpdateRecipeRequestNutrition;
  directions: UpdateRecipeRequestDirection[];
  tips: UpdateRecipeRequestTip[];
  subIngredients: UpdateRecipeRequestSubIngredient[];
}
