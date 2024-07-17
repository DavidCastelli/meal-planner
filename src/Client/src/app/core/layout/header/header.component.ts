import {Component, inject, OnInit} from '@angular/core';
import {AsyncPipe, NgOptimizedImage} from "@angular/common";
import {CommunicationService} from "../communication.service";
import {UserService} from "../../auth/services/user.service";
import {Observable} from "rxjs";
import {User} from "../../auth/user.model";

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    NgOptimizedImage,
    AsyncPipe
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit{
  private readonly communicationService = inject(CommunicationService);
  private readonly userService = inject(UserService);

  sidebarIsOpen = false;
  curUserInfo$!: Observable<User>;

  ngOnInit() {
    this.curUserInfo$ = this.userService.getUserInfo();
  }

  toggleSidebar() {
    this.sidebarIsOpen = !this.sidebarIsOpen;
    this.communicationService.sendToggleNotification(this.sidebarIsOpen);
  }
}
