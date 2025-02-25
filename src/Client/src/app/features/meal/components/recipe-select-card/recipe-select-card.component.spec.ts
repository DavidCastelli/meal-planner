import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecipeSelectCardComponent } from './recipe-select-card.component';

describe('RecipeSelectCardComponent', () => {
  let component: RecipeSelectCardComponent;
  let fixture: ComponentFixture<RecipeSelectCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecipeSelectCardComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(RecipeSelectCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
