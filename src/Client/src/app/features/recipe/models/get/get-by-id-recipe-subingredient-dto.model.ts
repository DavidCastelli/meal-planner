import {GetByIdRecipeIngredientDto} from "./get-by-id-recipe-ingredient-dto.model";

export interface GetByIdRecipeSubIngredientDto {
  Name?: string;
  Ingredients: GetByIdRecipeIngredientDto[];
}
