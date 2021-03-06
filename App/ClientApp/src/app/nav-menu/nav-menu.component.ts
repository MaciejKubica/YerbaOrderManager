import { Component } from '@angular/core';
import { DataService } from "../shared/dataService"
import { Role } from "../shared/role";
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;

  constructor(private data: DataService, public translate: TranslateService) {
    translate.addLangs(['en', 'pl']);
    translate.setDefaultLang('pl');

    const browserLang = translate.getBrowserLang();
    translate.use(browserLang.match(/en|pl/) ? browserLang : 'pl');
  }

  get hidden() : boolean {
    return this.data.loginRequired;
  }

  get isInRole(): boolean {
    return this.data.loggedUserRoles != null && this.data.loggedUserRoles.length > 0 && this.data.loggedUserRoles.some(this.isAdmin);
  }

  isAdmin(element, index, array) {
    return element === 'Administrator';
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
