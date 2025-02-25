import { Component, DestroyRef, inject, Input } from '@angular/core';
import { MealService } from '../../meal.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  CardComponent,
  CardType,
} from '../../../../shared/components/card/card.component';
import { GetMealsDto } from '../../models/get/get-meals-dto.model';
import { slideAnimation } from '../../../../shared/animations/slide.animation';
import { Router } from '@angular/router';
import { SidebarService } from '../../../../core/layout/sidebar.service';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-meals',
  standalone: true,
  imports: [CardComponent, AsyncPipe],
  animations: [slideAnimation],
  templateUrl: './meals.component.html',
  styleUrl: './meals.component.css',
})
export class MealsComponent {
  private readonly mealService = inject(MealService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly sidebarService = inject(SidebarService);

  protected readonly CardType = CardType;
  public readonly isSideBarOpen$ = this.sidebarService.openClose$;

  @Input() meals: GetMealsDto[] = [];

  create() {
    void this.router.navigate(['/manage/meals/create']);
  }

  deleteMeal(id: number) {
    this.mealService
      .deleteMeal(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((success) => {
        if (success) {
          const index = this.meals.findIndex((meal) => meal.id === id);
          this.meals.splice(index, 1);
        }
      });
  }
}
