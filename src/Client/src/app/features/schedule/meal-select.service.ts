import { Injectable } from '@angular/core';
import { MealCardData } from './meal-card-data';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MealSelectService {
  private readonly mealAddedSource = new Subject<MealCardData>();
  public readonly mealAdded$ = this.mealAddedSource.asObservable();
  private readonly mealRemovedSource = new Subject<MealCardData>();
  public readonly mealRemoved$ = this.mealRemovedSource.asObservable();

  setMealAdded(meal: MealCardData) {
    this.mealAddedSource.next(meal);
  }

  setMealRemoved(meal: MealCardData) {
    this.mealRemovedSource.next(meal);
  }
}
