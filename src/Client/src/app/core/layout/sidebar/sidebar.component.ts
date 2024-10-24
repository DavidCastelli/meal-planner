import {
  Component,
  DestroyRef,
  ElementRef,
  inject,
  OnInit,
} from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { filter, fromEvent } from 'rxjs';
import { SidebarService } from '../sidebar.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css',
})
export class SidebarComponent implements OnInit {
  private readonly sidebarService = inject(SidebarService);
  private readonly document = inject(DOCUMENT);
  private readonly elementRef = inject(ElementRef);
  private readonly destroyRef = inject(DestroyRef);

  ngOnInit() {
    fromEvent(this.document, 'click')
      .pipe(
        filter((event) => !this.isInside(event.target as HTMLElement)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(() => {
        this.sidebarService.setIsOpen(false);
      });
  }

  private isInside(element: HTMLElement): boolean {
    // Considers the sidebar button located in the header as part of the sidebar.
    // Needed to prevent animation bug when spam clicking the sidebar button.
    // Without this condition the sidebar is closed twice once for when the sidebar button is clicked and once for the click outside event.
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
