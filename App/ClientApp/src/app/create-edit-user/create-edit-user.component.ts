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
  }

  public barLabel: string;
  public roles: any[];

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
    });
  }

  getUserData(email: any) {
    this.data.getUserData(email).subscribe(success => {
      if (success) {
        this.usertoadd = this.data.userData;
        for (let item of this.usertoadd.roles) {
          this.roleSelections.find(x => x.id === item.id).isChecked = true;
        }
      }
    });
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
      });
    } else {
      this.data.createUser(this.usertoadd).subscribe(success => {
        if (!success) {
          console.log("Not created!");
        } else {
          this.router.navigate(["/users"]);
        }
      });
    }    
  }

}
