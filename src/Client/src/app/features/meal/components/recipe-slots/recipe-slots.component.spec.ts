import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecipeSlotsComponent } from './recipe-slots.component';

describe('RecipeSlotsComponent', () => {
  let component: RecipeSlotsComponent;
  let fixture: ComponentFixture<RecipeSlotsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecipeSlotsComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(RecipeSlotsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
