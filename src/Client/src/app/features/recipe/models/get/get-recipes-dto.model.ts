import { GetRecipesImageDto } from './get-recipes-image-dto.model';

export interface GetRecipesDto {
  id: number;
  title: string;
  image?: GetRecipesImageDto;
}
