import { Component } from '@angular/core';
import { DataService } from "../shared/dataService"

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;

  constructor(private data: DataService) { }

  get hidden() : boolean {
    return this.data.loginRequired;
  }

  get isInRole(): boolean {
    return this.data.loggedUserRoles != null && this.data.loggedUserRoles.length > 0 && this.data.loggedUserRoles.indexOf("Administrator") >= 0;
  }

  logoff() {
    this.data.logoff();
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
