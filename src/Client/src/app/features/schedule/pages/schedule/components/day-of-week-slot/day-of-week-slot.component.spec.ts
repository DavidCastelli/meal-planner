import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DayOfWeekSlotComponent } from './day-of-week-slot.component';

describe('DayOfWeekSlotComponent', () => {
  let component: DayOfWeekSlotComponent;
  let fixture: ComponentFixture<DayOfWeekSlotComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DayOfWeekSlotComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(DayOfWeekSlotComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
