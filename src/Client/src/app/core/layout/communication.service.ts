import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CommunicationService {
  private readonly toggleNotificationSource = new Subject<boolean>();
  readonly toggleNotification$ = this.toggleNotificationSource.asObservable();

  sendToggleNotification(isOpen: boolean) {
    this.toggleNotificationSource.next(isOpen);
  }
}
