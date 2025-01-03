import { Component, inject } from '@angular/core';
import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';

@Component({
  selector: 'app-notification-modal',
  standalone: true,
  imports: [],
  templateUrl: './notification-modal.component.html',
  styleUrl: './notification-modal.component.css',
})
export class NotificationModalComponent {
  private readonly dialogRef: DialogRef = inject(DialogRef);
  public readonly dialogData: { title: string; message: string } =
    inject(DIALOG_DATA);

  continue() {
    this.dialogRef.close();
  }
}
