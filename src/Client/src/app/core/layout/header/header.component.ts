import { Component, inject, OnInit } from '@angular/core';
import { AsyncPipe, NgOptimizedImage } from '@angular/common';
import { Observable } from 'rxjs';
import { AuthService } from '../../auth/auth.service';
import { Router } from '@angular/router';
import { UserInfo } from '../../auth/user-info.model';
import { SidebarService } from '../sidebar.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [NgOptimizedImage, AsyncPipe],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent implements OnInit {
  private readonly sidebarService = inject(SidebarService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  public curUserInfo$!: Observable<UserInfo | null>;
  public isSidebarOpen$!: Observable<boolean>;

  ngOnInit() {
    this.curUserInfo$ = this.authService.curUserInfo$;
    this.isSidebarOpen$ = this.sidebarService.toggleNotification$;
  }

  toggleSidebar() {
    this.sidebarService.sendToggleNotification();
  }

  logout() {
    this.router
      .navigate(['/landing'])
      .then((result) => {
        if (result) {
          this.authService.logout().subscribe();
        }
      })
      .catch((error) => console.log(error));
  }
}
