import { GetByIdRecipeIngredientDto } from './get-by-id-recipe-ingredient-dto.model';

export interface GetByIdRecipeSubIngredientDto {
  name?: string;
  ingredients: GetByIdRecipeIngredientDto[];
}
