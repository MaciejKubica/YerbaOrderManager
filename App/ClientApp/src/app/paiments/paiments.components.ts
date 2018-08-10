import { Component, OnInit } from "@angular/core"
import { DataService } from "../shared/dataService"
import { Router } from "@angular/router"
import { Order } from '../shared/order';
import { User } from '../shared/user';

import { PaimentRequest } from '../shared/paimentrequest';

@Component({
  selector: "app-paiments",
  templateUrl: "./paiments.component.html",
  styleUrls: []
})
export class PaimentsComponent implements OnInit {
  ngOnInit(): void {

    this.user = JSON.parse(localStorage.getItem("LoggedUser"));

    this.loadPaiments();
  }

  paiments: any[];
  user: User;

  constructor(private data: DataService, private router: Router) {
    
  }

  loadPaiments() {
    this.data.getPaimentsRequests().subscribe(success => {
      if (success) {
        
        this.paiments = this.data.paimentsRequests.filter(x => x.userId === this.user.id);
      }
    });
  }

}
