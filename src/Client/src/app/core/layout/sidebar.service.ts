import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SidebarService {
  private readonly openCloseSource = new BehaviorSubject<boolean>(false);
  public readonly openClose$ = this.openCloseSource.asObservable();
  private readonly animationDoneSource = new BehaviorSubject<boolean>(true);
  public readonly animationDone$ = this.animationDoneSource.asObservable();

  private isSidebarOpen = false;

  toggle() {
    this.isSidebarOpen = !this.isSidebarOpen;
    this.openCloseSource.next(this.isSidebarOpen);
  }

  getIsOpen() {
    return this.openCloseSource.value;
  }

  getIsAnimationDone() {
    return this.animationDoneSource.value;
  }

  setIsOpen(value: boolean) {
    this.isSidebarOpen = value;
    this.openCloseSource.next(value);
  }

  setIsAnimationDone(value: boolean) {
    this.animationDoneSource.next(value);
  }
}
