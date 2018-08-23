import { Component, OnInit } from "@angular/core"
import { DataService } from "../shared/dataService"
import { Router } from "@angular/router"
import { TranslateService, LangChangeEvent } from "@ngx-translate/core";

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: []
})
export class LoginComponent {

  constructor(private data: DataService, private router: Router, private translate: TranslateService) {}

  errorMessage: string = "";  

  public creds = {
    username: "",
    password: ""
  };

  onLogin() {
    this.data.login(this.creds)
      .subscribe(success => {
        if (success) {

          this.data.getLoggedUserData().subscribe(dataSuccess => {
            if (dataSuccess) {
              this.router.navigate(["/userpanel"]);
            }
          });
        }
        },
      err => this.errorMessage = this.translate.instant("GLOBAL.FAILEDTOLOGIN"));

  }
}
