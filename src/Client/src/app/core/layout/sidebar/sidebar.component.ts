import {
  Component,
  DestroyRef,
  ElementRef,
  HostListener,
  inject,
  OnInit,
} from '@angular/core';
import { animate, style, transition, trigger } from '@angular/animations';
import { AsyncPipe, DOCUMENT } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { filter, fromEvent, Observable, Subscription, tap } from 'rxjs';
import { SidebarService } from '../sidebar.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [AsyncPipe, RouterLink, RouterLinkActive],
  animations: [
    trigger('openClose', [
      transition(':enter', [
        style({ transform: 'translateX(-100%)' }),
        animate('300ms ease-in'),
      ]),
      transition(':leave', [
        animate('300ms ease-out', style({ transform: 'translateX(-100%)' })),
      ]),
    ]),
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css',
})
export class SidebarComponent implements OnInit {
  private readonly sidebarService = inject(SidebarService);
  private readonly document = inject(DOCUMENT);
  private readonly elementRef = inject(ElementRef);
  private readonly destroyRef = inject(DestroyRef);
  private subscription = new Subscription();

  public isOpen$!: Observable<boolean>;
  public clickOutside$!: Observable<Event>;

  @HostListener('window:keydown.alt.q')
  openShortcut() {
    this.sidebarService.sendToggleNotification();
  }

  ngOnInit() {
    this.clickOutside$ = fromEvent(this.document, 'click').pipe(
      filter((event) => !this.isInside(event.target as HTMLElement)),
      takeUntilDestroyed(this.destroyRef),
    );

    this.isOpen$ = this.sidebarService.toggleNotification$.pipe(
      tap((isOpen) => {
        if (isOpen) {
          this.subscription = this.clickOutside$.subscribe(() => {
            this.sidebarService.sendToggleNotification();
          });
        } else {
          this.subscription.unsubscribe();
        }
      }),
    );
  }

  closeAfterSelect() {
    this.sidebarService.sendToggleNotification();
  }

  private isInside(element: HTMLElement): boolean {
    // The sidebar button is inside the header and triggers a click outside event immediately after the sidebar opens.
    // This condition is needed so that the sidebar does not immediately close after the sidebar button is clicked.
    if (
      element instanceof HTMLImageElement &&
      element.id === 'sidebar-button-img'
    ) {
      return true;
    }

    return (
      element === this.elementRef.nativeElement ||
      this.elementRef.nativeElement.contains(element)
    );
  }
}
