import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { RecipeCardData } from './recipe-card-data';

@Injectable({
  providedIn: 'root',
})
export class RecipeSelectService {
  private readonly recipeSelectOpenSource = new BehaviorSubject<boolean>(false);
  public readonly recipeSelectOpen$ =
    this.recipeSelectOpenSource.asObservable();
  private readonly recipeRemovedSource = new Subject<RecipeCardData>();
  public readonly recipeRemoved$ = this.recipeRemovedSource.asObservable();
  private readonly animationDoneSource = new BehaviorSubject<boolean>(true);
  public readonly animationDone$ = this.animationDoneSource.asObservable();

  private isRecipeSelectOpen = false;

  toggleRecipeSelect() {
    this.isRecipeSelectOpen = !this.isRecipeSelectOpen;
    this.recipeSelectOpenSource.next(this.isRecipeSelectOpen);
  }

  setIsOpen(isOpen: boolean) {
    this.isRecipeSelectOpen = isOpen;
    this.recipeSelectOpenSource.next(this.isRecipeSelectOpen);
  }

  setRecipeRemoved(recipe: RecipeCardData) {
    this.recipeRemovedSource.next(recipe);
  }

  setIsAnimationDone(value: boolean) {
    this.animationDoneSource.next(value);
  }
}
