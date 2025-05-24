import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MealCardData } from '../../../../meal-card-data';
import { ScheduledMealCardComponent } from '../scheduled-meal-card/scheduled-meal-card.component';
import { ScheduledMealPlaceholderComponent } from '../scheduled-meal-placeholder/scheduled-meal-placeholder.component';
import { DayOfWeekSlotComponent } from '../day-of-week-slot/day-of-week-slot.component';
import { CdkDragDrop, CdkDropList } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-day-of-week',
  standalone: true,
  imports: [
    ScheduledMealCardComponent,
    ScheduledMealPlaceholderComponent,
    DayOfWeekSlotComponent,
    CdkDropList,
  ],
  templateUrl: './day-of-week.component.html',
  styleUrl: './day-of-week.component.css',
})
export class DayOfWeekComponent {
  public showPreview = false;

  @Input({ required: true }) day!: string;
  @Input() mealData?: MealCardData;

  @Output() deleted = new EventEmitter<number>();
  @Output() schedule = new EventEmitter<{
    day: string;
    mealCardData: MealCardData;
  }>();

  remove(id: number) {
    this.deleted.emit(id);
  }

  entered() {
    this.showPreview = true;
  }

  exited() {
    this.showPreview = false;
  }

  emptyPredicate = () => {
    return this.mealData === undefined;
  };

  dropped(event: CdkDragDrop<MealCardData>) {
    this.showPreview = false;
    const data = event.item.data as MealCardData;
    this.schedule.emit({ day: this.day, mealCardData: data });
  }
}
