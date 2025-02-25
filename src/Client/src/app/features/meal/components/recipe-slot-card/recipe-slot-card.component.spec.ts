import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecipeSlotCardComponent } from './recipe-slot-card.component';

describe('RecipeSlotCardComponent', () => {
  let component: RecipeSlotCardComponent;
  let fixture: ComponentFixture<RecipeSlotCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecipeSlotCardComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(RecipeSlotCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
