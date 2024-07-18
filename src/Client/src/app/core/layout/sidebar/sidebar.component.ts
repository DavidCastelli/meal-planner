import { Component, inject, OnInit } from '@angular/core';
import { animate, style, transition, trigger } from '@angular/animations';
import { CommunicationService } from '../communication.service';
import { AsyncPipe } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { Observable } from 'rxjs';

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
  private readonly communicationService = inject(CommunicationService);
  isOpen$!: Observable<boolean>;

  ngOnInit() {
    this.isOpen$ = this.communicationService.toggleNotification$;
  }
}
