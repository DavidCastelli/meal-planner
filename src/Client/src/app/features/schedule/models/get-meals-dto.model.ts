import { GetMealsImageDto } from './get-meals-image-dto.model';
import { Schedule } from '../../../shared/enums/schedule.enum';

export interface GetMealsDto {
  id: number;
  title: string;
  image?: GetMealsImageDto;
  schedule: Schedule;
}
