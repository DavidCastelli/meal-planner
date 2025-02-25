import { Component, inject, Input, OnInit } from '@angular/core';
import { RecipeSlotComponent } from '../recipe-slot/recipe-slot.component';
import {
  CdkDrag,
  CdkDragDrop,
  CdkDropList,
  transferArrayItem,
} from '@angular/cdk/drag-drop';
import { RecipeSelectService } from '../../recipe-select.service';
import { AsyncPipe } from '@angular/common';
import { FormControl } from '@angular/forms';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { GetMealByIdRecipeDto } from '../../models/get/get-meal-by-id-recipe-dto.model';
import { RecipeCardData } from '../../recipe-card-data';

@Component({
  selector: 'app-recipe-slots',
  standalone: true,
  imports: [RecipeSlotComponent, CdkDropList, AsyncPipe, ControlErrorComponent],
  templateUrl: './recipe-slots.component.html',
  styleUrl: './recipe-slots.component.css',
})
export class RecipeSlotsComponent implements OnInit {
  private readonly recipeSelectService = inject(RecipeSelectService);

  public readonly isAnimationDone$ = this.recipeSelectService.animationDone$;

  public readonly recipeSlots1: RecipeCardData[] = [];
  public readonly recipeSlots2: RecipeCardData[] = [];
  public readonly recipeSlots3: RecipeCardData[] = [];

  @Input({ required: true }) recipeIds!: FormControl<number[]>;
  @Input() initialRecipes?: GetMealByIdRecipeDto[];

  ngOnInit() {
    if (this.initialRecipes) {
      const initialRecipe1 = this.initialRecipes.at(0);
      if (initialRecipe1) {
        this.recipeSlots1.push({
          id: initialRecipe1.id,
          title: initialRecipe1.title,
          imageUrl: initialRecipe1.imageUrl,
        });
      }

      const initialRecipe2 = this.initialRecipes.at(1);
      if (initialRecipe2) {
        this.recipeSlots2.push({
          id: initialRecipe2.id,
          title: initialRecipe2.title,
          imageUrl: initialRecipe2.imageUrl,
        });
      }

      const initialRecipe3 = this.initialRecipes.at(2);
      if (initialRecipe3) {
        this.recipeSlots3.push({
          id: initialRecipe3.id,
          title: initialRecipe3.title,
          imageUrl: initialRecipe3.imageUrl,
        });
      }
    }
  }

  get recipeSlot1() {
    return this.recipeSlots1.at(0);
  }

  get recipeSlot2() {
    return this.recipeSlots2.at(0);
  }

  get recipeSlot3() {
    return this.recipeSlots3.at(0);
  }

  drop(event: CdkDragDrop<RecipeCardData[], RecipeCardData[]>) {
    if (event.previousContainer !== event.container) {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex,
      );

      const recipe = event.container.data.at(0);
      if (recipe) {
        this.recipeIds.value.push(recipe.id);
        this.recipeIds.markAsDirty();
        this.recipeIds.updateValueAndValidity();
      } else {
        console.error('Could not retrieve dropped recipe data.');
      }
    }
  }

  emptyPredicate(drag: CdkDrag, drop: CdkDropList<RecipeCardData[]>) {
    return drop.data.length === 0;
  }

  select() {
    this.recipeSelectService.toggleRecipeSelect();
  }

  remove(slots: RecipeCardData[]) {
    const removedRecipe = slots.shift();
    if (removedRecipe) {
      this.recipeSelectService.setRecipeRemoved(removedRecipe);
      const indexToRemove = this.recipeIds.value.findIndex((value) => {
        return value === removedRecipe.id;
      });
      this.recipeIds.value.splice(indexToRemove, 1);
      this.recipeIds.markAsDirty();
      this.recipeIds.updateValueAndValidity();
    }
  }
}
