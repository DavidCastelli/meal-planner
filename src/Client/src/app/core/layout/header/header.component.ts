import { Component, inject, OnInit } from '@angular/core';
import { AsyncPipe, NgOptimizedImage } from '@angular/common';
import { AuthService } from '../../auth/auth.service';
import { Router } from '@angular/router';
import { SidebarService } from '../sidebar.service';
import { HeaderService } from '../header.service';

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
  private readonly headerService = inject(HeaderService);

  public readonly curUserInfo$ = this.authService.curUserInfo$;
  public readonly isSidebarOpen$ = this.sidebarService.openClose$;
  public readonly title$ = this.headerService.title$;

  ngOnInit() {
    this.headerService.updateTitle();
  }

  toggleSidebar() {
    const isSidebarAnimationDone = this.sidebarService.getIsAnimationDone();
    if (!isSidebarAnimationDone) {
      return;
    }
    this.sidebarService.toggle();
  }

  logout() {
    this.router
      .navigate(['/landing'])
      .then((success) => {
        if (success) {
          this.sidebarService.setIsOpen(false); // Needed so sidebar does not open after logging back in
          this.authService.logout().subscribe();
        }
      })
      .catch((error) => console.error(error));
  }
}
