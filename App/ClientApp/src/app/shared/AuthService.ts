import { Injectable } from '@angular/core';
import { Route, Router, CanActivate, CanLoad } from '@angular/router';
import { DataService } from "../shared/dataService";
import { Observable } from 'rxjs/Observable';

@Injectable()
export class AuthService implements CanActivate, CanLoad {

  canLoad(route: Route): boolean | Observable<boolean> | Promise<boolean> {
    if (this.data.loginRequired) {
      this.router.navigate(['/login']);
      return false;
    }
    return true;
  }
  constructor(public data: DataService, public router: Router) { }

  canActivate(): boolean {
    if (this.data.loginRequired) {
      this.router.navigate(['/login']);
      return false;
    }
    return true;
  }

}
