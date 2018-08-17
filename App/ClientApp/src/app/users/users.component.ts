import { Component, OnInit } from "@angular/core"
import { DataService } from "../shared/dataService"
import { Router } from "@angular/router"
import { User } from '../shared/user'

@Component({
  selector: "app-users",
  templateUrl: "./users.component.html",
  styleUrls: []
})

export class UsersComponent implements OnInit {

  constructor(private data: DataService, private router: Router) {

  }

  public users = [];

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers() {
    this.data.loadUsers().subscribe(success => {
      if (success) {
        this.users = this.data.users.filter(x => x.isDeleted == false);
      } else {
        console.log("Not loaded");
      }
    });
  }

  createUser() {
    this.router.navigate(["/create-edit-user"]);
  }


  editUser(user: User) {
    console.log(user.email + " " + user.name + " " + user.password);
    this.router.navigate(["/create-edit-user"], { queryParams: { email: user.email } });
  }

  lockUser(user: User) {
    user.lockoutEnabled = true;
    user.lockoutEndDateUtc = new Date(2020, 12);
    this.data.updateUser(user).subscribe(success => {
      if (!success) {
        console.log("Not able to edit!");
      } else {
        this.loadUsers();
      }
    });
  }

  deleteUser(user: User) {
    console.log(user.email + " " + user.name + " " + user.password);
    this.data.deleteUser(user).subscribe(success => {
      if (!success) {
        console.log("Not deleted!");
      } else {
        this.loadUsers();
      }
    });
  }  
}

