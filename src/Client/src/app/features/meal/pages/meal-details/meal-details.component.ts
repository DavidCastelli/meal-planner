import { Component, inject, Input, OnInit } from '@angular/core';
import { GetMealByIdDto } from '../../models/get/get-meal-by-id-dto.model';
import { TagType } from '../../tag-type.enum';
import { slideAnimation } from '../../../../shared/animations/slide.animation';
import { SidebarService } from '../../../../core/layout/sidebar.service';
import { AsyncPipe, NgOptimizedImage } from '@angular/common';
import { GetMealByIdRecipeDetailsDto } from '../../models/get/get-meal-by-id-recipe-details-dto.model';
import { GetMealByIdRecipeNutritionDto } from '../../models/get/get-meal-by-id-recipe-nutrition-dto.model';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-meal-details',
  standalone: true,
  imports: [AsyncPipe, NgOptimizedImage, RouterLink],
  animations: [slideAnimation],
  templateUrl: './meal-details.component.html',
  styleUrl: './meal-details.component.css',
})
export class MealDetailsComponent implements OnInit {
  private readonly sidebarService = inject(SidebarService);

  protected readonly TagType = TagType;
  public readonly isSideBarOpen$ = this.sidebarService.openClose$;
  public recipesDetails!: GetMealByIdRecipeDetailsDto;
  public recipesNutrition!: GetMealByIdRecipeNutritionDto;

  @Input() meal!: GetMealByIdDto;

  ngOnInit() {
    let prepTime: number | undefined = undefined;
    let cookTime: number | undefined = undefined;
    let servings: number | undefined = undefined;
    let calories: number | undefined = undefined;
    let fat: number | undefined = undefined;
    let carbs: number | undefined = undefined;
    let protein: number | undefined = undefined;

    for (const recipe of this.meal.recipes) {
      if (recipe.details.prepTime !== undefined) prepTime = 0;
      if (recipe.details.cookTime !== undefined) cookTime = 0;
      if (recipe.details.servings !== undefined) servings = 0;
      if (recipe.nutrition.calories !== undefined) calories = 0;
      if (recipe.nutrition.fat !== undefined) fat = 0;
      if (recipe.nutrition.carbs !== undefined) carbs = 0;
      if (recipe.nutrition.protein !== undefined) protein = 0;
    }

    for (const recipe of this.meal.recipes) {
      if (prepTime !== undefined) prepTime += recipe.details.prepTime ?? 0;
      if (cookTime !== undefined) cookTime += recipe.details.cookTime ?? 0;
      if (servings !== undefined) servings += recipe.details.servings ?? 0;
      if (calories !== undefined) calories += recipe.nutrition.calories ?? 0;
      if (fat !== undefined) fat += recipe.nutrition.fat ?? 0;
      if (carbs !== undefined) carbs += recipe.nutrition.carbs ?? 0;
      if (protein !== undefined) protein += recipe.nutrition.protein ?? 0;
    }

    this.recipesDetails = { prepTime, cookTime, servings };
    this.recipesNutrition = { calories, fat, carbs, protein };
  }
}
