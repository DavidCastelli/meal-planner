import { Component, DestroyRef, inject, Input } from '@angular/core';
import { Router } from '@angular/router';
import { GetRecipesDto } from '../../models/get/get-recipes-dto.model';
import {
  CardComponent,
  CardType,
} from '../../../../shared/components/card/card.component';
import { slideAnimation } from '../../../../shared/animations/slide.animation';
import { SidebarService } from '../../../../core/layout/sidebar.service';
import { AsyncPipe } from '@angular/common';
import { RecipeService } from '../../recipe.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-recipes',
  standalone: true,
  imports: [CardComponent, AsyncPipe],
  animations: [slideAnimation],
  templateUrl: './recipes.component.html',
  styleUrl: './recipes.component.css',
})
export class RecipesComponent {
  private readonly recipeService = inject(RecipeService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly sidebarService = inject(SidebarService);

  protected readonly CardType = CardType;
  public readonly isSideBarOpen$ = this.sidebarService.openClose$;

  @Input() recipes: GetRecipesDto[] = [];

  create() {
    void this.router.navigate(['/manage/recipes/create']);
  }

  deleteRecipe(id: number) {
    this.recipeService
      .deleteRecipe(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((success) => {
        if (success) {
          const index = this.recipes.findIndex((recipe) => recipe.id === id);
          this.recipes.splice(index, 1);
        }
      });
  }
}
