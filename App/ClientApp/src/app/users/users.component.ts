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
        this.users = this.data.users;
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

