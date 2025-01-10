import { Component, inject, Input, OnInit } from '@angular/core';
import { GetRecipeByIdDto } from '../../models/get/get-recipe-by-id-dto.model';
import { slideAnimation } from '../../../../shared/animations/slide.animation';
import { SidebarService } from '../../../../core/layout/sidebar.service';
import { AsyncPipe, NgOptimizedImage } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-recipe-details',
  standalone: true,
  imports: [AsyncPipe, NgOptimizedImage, RouterLink],
  animations: [slideAnimation],
  templateUrl: './recipe-details.component.html',
  styleUrl: './recipe-details.component.css',
})
export class RecipeDetailsComponent implements OnInit {
  private readonly sidebarService = inject(SidebarService);
  public readonly isSideBarOpen$ = this.sidebarService.openClose$;
  public subIngredientsGridColumnStyle!: string;

  @Input() recipe!: GetRecipeByIdDto;

  ngOnInit(): void {
    const subIngredientCount = this.recipe.subIngredients.length;
    const subIngredientsColumns = Math.min(subIngredientCount * 2, 6);
    const emptyColumns = 12 - subIngredientsColumns - 6; // Always even

    const subIngredientsColumnStart = emptyColumns / 2; // 0 | 2 | 4
    const subIngredientsColumnEnd =
      subIngredientsColumnStart + subIngredientsColumns;

    this.subIngredientsGridColumnStyle = `${subIngredientsColumnStart + 1} / ${subIngredientsColumnEnd + 1}`;
  }
}
