import {
  Component,
  DestroyRef,
  EventEmitter,
  HostListener,
  inject,
  Input,
  Output,
} from '@angular/core';
import { NgOptimizedImage } from '@angular/common';
import { CdkMenu, CdkMenuItem, CdkMenuTrigger } from '@angular/cdk/menu';
import { Router } from '@angular/router';
import { ModalService } from '../../services/modal.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { exhaustMap, filter, map } from 'rxjs';
import { RecipeService } from '../../../features/recipe/recipe.service';

export enum CardType {
  MEAL = 'meal',
  RECIPE = 'recipe',
}

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [NgOptimizedImage, CdkMenu, CdkMenuTrigger, CdkMenuItem],
  templateUrl: './card.component.html',
  styleUrl: './card.component.css',
})
export class CardComponent {
  private readonly recipeService = inject(RecipeService);
  private readonly router = inject(Router);
  private readonly modalService = inject(ModalService);
  private readonly destroyRef = inject(DestroyRef);

  @Input({ required: true }) type!: CardType;
  @Input({ required: true }) id!: number;
  @Input({ required: true }) title!: string;
  @Input() imageUrl?: string | undefined;

  @Output() recipeDeleted = new EventEmitter<number>();

  @HostListener('click', ['$event'])
  showRecipeDetails(event: Event) {
    // Prevents the cards action button from triggering navigation to details page.
    if (event.target instanceof HTMLButtonElement) {
      return;
    }

    void this.router.navigate([`/manage/${this.type}s/${this.id}/details`]);
  }

  update() {
    void this.router.navigate([`/manage/${this.type}s/${this.id}/edit`]);
  }

  delete() {
    const title = 'Delete';
    const message =
      'Are you sure you want to delete this recipe? All data for the current recipe will be lost.';

    this.modalService
      .openConfirmationModal(title, message)
      .pipe(
        map((result) => {
          if (result === undefined) {
            return false;
          } else {
            return result;
          }
        }),
        filter((result) => result),
        exhaustMap(() => this.recipeService.deleteRecipe(this.id)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(() => {
        this.recipeDeleted.emit(this.id);
      });
  }
}
