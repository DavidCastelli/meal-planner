import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RecipeService } from '../../recipe.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { GetRecipesDto } from '../../models/get/get-recipes-dto.model';
import { CardComponent } from '../../../../shared/components/card/card.component';
import { slideAnimation } from '../../../../shared/animations/slide.animation';
import { SidebarService } from '../../../../core/layout/sidebar.service';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-recipes',
  standalone: true,
  imports: [CardComponent, AsyncPipe],
  animations: [slideAnimation],
  templateUrl: './recipes.component.html',
  styleUrl: './recipes.component.css',
})
export class RecipesComponent implements OnInit {
  private readonly recipeService = inject(RecipeService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly sidebarService = inject(SidebarService);

  public readonly isSideBarOpen$ = this.sidebarService.openClose$;
  public recipes: GetRecipesDto[] = [];

  ngOnInit() {
    this.getRecipes();
  }

  getRecipes() {
    this.recipeService
      .getRecipes()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((recipes) => {
        this.recipes = recipes;
      });
  }

  create() {
    void this.router.navigate(['/manage/recipes/create']);
  }
}
