import { inject, Injectable } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class HeaderService {
  private readonly title = inject(Title);
  private readonly titleSource = new BehaviorSubject<string>('');
  public readonly title$ = this.titleSource.asObservable();

  updateTitle() {
    const title = this.title.getTitle();
    this.titleSource.next(title);
  }
}
