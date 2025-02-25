import { TestBed } from '@angular/core/testing';
import { ResolveFn } from '@angular/router';

import { tagsResolver } from './tags.resolver';

describe('tagsResolver', () => {
  const executeResolver: ResolveFn<boolean> = (...resolverParameters) =>
    TestBed.runInInjectionContext(() => tagsResolver(...resolverParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeResolver).toBeTruthy();
  });
});
