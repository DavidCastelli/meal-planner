import { GetMealsImageDto } from './get-meals-image-dto.model';

export interface GetMealsDto {
  id: number;
  title: string;
  image?: GetMealsImageDto;
}
