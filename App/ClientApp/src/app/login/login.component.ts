import { Component, OnInit } from "@angular/core"
import { DataService } from "../shared/dataService"
import { Router } from "@angular/router"

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: []
})
export class LoginComponent {

  constructor(private data: DataService, private router: Router) {}

  errorMessage: string="";

  public creds = {
    username: "",
    password: ""
  };

  onLogin() {
    this.data.login(this.creds)
      .subscribe(success => {
        if (success) {

          this.data.getLoggedUserData().subscribe(success => {
          });

          this.router.navigate(["/userpanel"]);
        }
        },
      err => this.errorMessage = "Failed to login ");

  }
}
