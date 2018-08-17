import { Component, OnInit } from "@angular/core"
import { DataService } from "../shared/dataService"
import { Router, ActivatedRoute } from "@angular/router"
import { PasswordStrengthBar } from '../passwordStrengthBar';
import { User } from '../shared/user';
import {RoleSelection} from "../shared/roleSection";

@Component({
  selector: "app-create-edit-user",
  templateUrl: "./create-edit-user.component.html",
  styleUrls: []
})

export class CreateEditUserComponent implements OnInit {

  constructor(private data: DataService, private router: Router, private activatedRouter: ActivatedRoute) {

    this.usertoadd = new User();
    this.barLabel = "Password strength:";
    this.currentUser = JSON.parse(localStorage.getItem("LoggedUser"));
  }

  public barLabel: string;
  public roles: any[];
  public currentUser: User;
  public errorMessage: string;

  public patternDef = "^[a-z\d-_\s]+$";

  public roleSelections: RoleSelection[];

  ngOnInit(): void {
    if (this.activatedRouter.queryParams) {
      this.activatedRouter.queryParams.subscribe(params => {
        if (params["email"]) {
          this.getUserData(params["email"]);          
          this.openForEdit = true;
        }
      }, error => console.log(error));          
    }

    this.getRoles();
  }

  getRoles() {
    this.data.getRoles().subscribe(success => {
      if (success) {
        this.roles = this.data.rolesInSystem;
        this.roleSelections = this.data.rolesInSystem;       
      }
    }, error => this.errorMessage = error);
  }

  getUserData(email: any) {
    this.data.getUserData(email).subscribe(success => {
      if (success) {
        this.usertoadd = this.data.userData;
        for (let item of this.usertoadd.roles) {
          this.roleSelections.find(x => x.id === item.id).isChecked = true;
        }
      }
    }, error => this.errorMessage = error);
  }

  public openForEdit: boolean = false;

  public usertoadd: User;

  onCreate() {

    this.usertoadd.roles = this.roleSelections.filter(x => x.isChecked == true);

    console.log(this.usertoadd.email);
    if (this.openForEdit) {
      this.data.updateUser(this.usertoadd).subscribe(success => {
        if (!success) {
          console.log("Not edited!");
        } else {
          this.router.navigate(["/users"]);
        }
      }, error => this.errorMessage = error);
    } else {
      this.data.createUser(this.usertoadd).subscribe(success => {
        if (!success) {
          console.log("Not created!");
        } else {
          this.router.navigate(["/users"]);
        }
      }, error => this.errorMessage = error);
    }    
  }

}
