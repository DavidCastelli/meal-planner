import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { AsyncPipe, NgOptimizedImage } from '@angular/common';
import { CommunicationService } from '../communication.service';
import { Observable } from 'rxjs';
import { AuthService } from '../../auth/auth.service';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UserInfo } from '../../auth/user-info.model';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [NgOptimizedImage, AsyncPipe],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent implements OnInit {
  private readonly communicationService = inject(CommunicationService);
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);

  public sidebarIsOpen = false;
  public curUserInfo$!: Observable<UserInfo | null>;

  ngOnInit() {
    this.curUserInfo$ = this.authService.curUserInfo$;
  }

  toggleSidebar() {
    this.sidebarIsOpen = !this.sidebarIsOpen;
    this.communicationService.sendToggleNotification(this.sidebarIsOpen);
  }

  logout() {
    this.authService
      .logout()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: () => void this.router.navigate(['/landing']) });
  }
}
