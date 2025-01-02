import { inject, Injectable } from '@angular/core';
import { Dialog } from '@angular/cdk/dialog';
import { ConfirmModalComponent } from '../components/confirm-modal/confirm-modal.component';
import { Observable } from 'rxjs';
import { NotificationModalComponent } from '../components/notification-modal/notification-modal.component';

@Injectable({
  providedIn: 'root',
})
export class ModalService {
  private readonly dialog = inject(Dialog);

  openConfirmationModal(
    title: string,
    message: string,
  ): Observable<boolean | undefined> {
    const dialogRef = this.dialog.open<boolean>(ConfirmModalComponent, {
      data: { title: title, message: message },
      maxWidth: '33%',
    });
    return dialogRef.closed;
  }

  openNotificationModal(title: string, message?: string): void {
    this.dialog.open(NotificationModalComponent, {
      data: { title: title, message: message },
      maxWidth: '33%',
    });
  }
}
