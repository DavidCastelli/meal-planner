import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';
import { delay } from 'rxjs';

@Component({
  selector: 'app-logout',
  standalone: true,
  imports: [],
  templateUrl: './logout.component.html',
  styleUrl: './logout.component.css',
})
export class LogoutComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  public canContinue = false;
  public logoutStatus = 'Logging out please wait...';

  ngOnInit() {
    this.authService
      .logout()
      .pipe(delay(2000))
      .subscribe((success) => {
        if (success) {
          this.logoutStatus = 'You have logged out successfully.';
        } else {
          this.logoutStatus =
            'Something went wrong, you may not have been logged out successfully.';
        }
        this.canContinue = true;
      });
  }

  continue() {
    void this.router.navigate(['/landing']);
  }
}
