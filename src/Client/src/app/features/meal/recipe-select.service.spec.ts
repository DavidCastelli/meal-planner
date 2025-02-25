import { TestBed } from '@angular/core/testing';

import { RecipeSelectService } from './recipe-select.service';

describe('RecipeSelectService', () => {
  let service: RecipeSelectService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RecipeSelectService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
