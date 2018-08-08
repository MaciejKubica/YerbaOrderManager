import { Component, OnInit } from "@angular/core"
import { DataService } from "../shared/dataService"
import { Router, ActivatedRoute } from "@angular/router"
import { PasswordStrengthBar } from '../passwordStrengthBar';
import { User } from '../shared/user';

@Component({
  selector: "app-register",
  templateUrl: "./register.component.html",
  styleUrls: []
})

export class RegisterComponent implements OnInit {

  constructor(private data: DataService, private router: Router, private activatedRouter: ActivatedRoute) {
    this.barLabel = "Password strength:";
    this.creds = new User();
  }

  public barLabel: string;

  public errorMessage: string;

  public creds: User;
 

  ngOnInit(): void {
   
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
