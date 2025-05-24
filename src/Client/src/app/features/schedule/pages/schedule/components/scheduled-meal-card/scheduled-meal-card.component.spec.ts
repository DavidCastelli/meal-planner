import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduledMealCardComponent } from './scheduled-meal-card.component';

describe('ScheduledMealCardComponent', () => {
  let component: ScheduledMealCardComponent;
  let fixture: ComponentFixture<ScheduledMealCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScheduledMealCardComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ScheduledMealCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
