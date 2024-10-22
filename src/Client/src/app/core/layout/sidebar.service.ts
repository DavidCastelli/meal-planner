import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SidebarService {
  private readonly toggleNotificationSource = new Subject<boolean>();
  public readonly toggleNotification$ =
    this.toggleNotificationSource.asObservable();

  private isSidebarOpen = false;

  sendToggleNotification() {
    this.isSidebarOpen = !this.isSidebarOpen;
    this.toggleNotificationSource.next(this.isSidebarOpen);
  }
}
