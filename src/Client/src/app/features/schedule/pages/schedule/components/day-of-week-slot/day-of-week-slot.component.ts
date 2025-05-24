import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-day-of-week-slot',
  standalone: true,
  imports: [],
  templateUrl: './day-of-week-slot.component.html',
  styleUrl: './day-of-week-slot.component.css',
})
export class DayOfWeekSlotComponent {
  @Input({ required: true }) showPreview!: boolean;
}
