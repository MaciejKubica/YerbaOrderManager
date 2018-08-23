import { Component, OnInit } from "@angular/core"
import { DataService } from "../shared/dataService"
import { Router, ActivatedRoute } from "@angular/router"
import { PasswordStrengthBar } from '../passwordStrengthBar';
import { User } from '../shared/user';
import { TranslateService, LangChangeEvent } from "@ngx-translate/core";

@Component({
  selector: "app-register",
  templateUrl: "./register.component.html",
  styleUrls: []
})

export class RegisterComponent implements OnInit {

  constructor(private data: DataService, private router: Router, private activatedRouter: ActivatedRoute, private translate: TranslateService) {

    this.barLabel = this.translate.instant("GLOBAL.PASSWORDSTREANGTH");

    this.creds = new User();
  }

  public barLabel: string;

  public errorMessage: string;

  public creds: User;
 

  ngOnInit(): void {
    this.translate.get("GLOBAL.PASSWORDSTREANGTH").subscribe(
      data => {
        this.barLabel = data;
      });

    this.translate.onLangChange.subscribe((event: LangChangeEvent) => {
      //this.barLabel = this.translate.instant("GLOBAL.PASSWORDSTREANGTH");
      console.log(this.translate.instant("GLOBAL.PASSWORDSTREANGTH"));
      this.barLabel = this.translate.instant("GLOBAL.PASSWORDSTREANGTH");
    });
  }

  onRegister() {

    this.data.registerUser(this.creds).subscribe(success => {
        if (success) {
          this.router.navigate(["/userpanel"]);
        }
      },
      err => this.errorMessage = "Failed to register account");
  }

}
