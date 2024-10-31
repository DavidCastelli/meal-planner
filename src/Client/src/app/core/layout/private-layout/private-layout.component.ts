import {
  Component,
  DestroyRef,
  HostListener,
  inject,
  OnInit,
} from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../header/header.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { filter } from 'rxjs';
import { SidebarService } from '../sidebar.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AsyncPipe } from '@angular/common';
import { animate, style, transition, trigger } from '@angular/animations';
import { ErrorService } from '../../errors/error.service';

@Component({
  selector: 'app-private-layout',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, SidebarComponent, AsyncPipe],
  animations: [
    trigger('openClose', [
      transition(':enter', [
        style({ transform: 'translateX(-120%)' }),
        animate('600ms 100ms ease-in'),
      ]),
      transition(':leave', [
        animate(
          '600ms 100ms ease-in',
          style({ transform: 'translateX(-120%)' }),
        ),
      ]),
    ]),
  ],
  templateUrl: './private-layout.component.html',
  styleUrl: './private-layout.component.css',
})
export class PrivateLayoutComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly sidebarService = inject(SidebarService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly errorService = inject(ErrorService);

  public readonly isSideBarOpen$ = this.sidebarService.openClose$;

  @HostListener('window:keydown.alt.q')
  sidebarToggleShortcut() {
    const isSidebarAnimationDone = this.sidebarService.getIsAnimationDone();
    if (!isSidebarAnimationDone) {
      return;
    }
    this.sidebarService.toggle();
  }

  ngOnInit() {
    this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(() => {
        this.errorService.clear();
        this.sidebarService.setIsOpen(false);
      });
  }

  sidebarAnimationStart() {
    this.sidebarService.setIsAnimationDone(false);
  }

  sidebarAnimationDone() {
    this.sidebarService.setIsAnimationDone(true);
  }
}
