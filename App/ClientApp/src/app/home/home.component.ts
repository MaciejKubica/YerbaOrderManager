import { Component, OnInit } from '@angular/core';
import { DataService } from "../shared/dataService"
import { User } from "../shared/user"
import { Router } from "@angular/router"
import { Order } from '../shared/order';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {


  ngOnInit(): void {
    this.loadAllOrders();
    if (!this.loginRequired) {
      this.getUserData();
    }
  }

  constructor(private data: DataService, private router: Router) {
    this.currentUser = new User();
  }

  public orders = [];
  currentUser: User;
  

  hideCloseOrder(userMadeBy: string): boolean {
    return userMadeBy === this.data.loggedUserEmail;
  }

  get loginRequired(): boolean {
    return this.data.loginRequired;
  }

  getUserData() {
    this.data.getLoggedUserData().subscribe(success => {
      if (success) {
        this.currentUser = this.data.userData;
      }
    });
  }

  closeOrder(order: Order) {
    this.data.closeOrder(order.id).subscribe(success => {
      if (success)
        this.loadAllOrders();
    });
  }

  loadAllOrders() {
    this.data.loadAllOrders().subscribe(success => {
      if (success) {
        this.orders = this.data.orders.filter(x => x.isPaid === false && x.isClosed === false);
      } else {
        console.log("Not loaded");
      }
    });
  }

  addOrderItem(order: Order) {
    this.router.navigate(["/addyerbatoorder"], { queryParams: { id: order.id } });
  }

  showSummary(order: Order) {
    this.router.navigate(["/showsummary"], { queryParams: { id: order.id } });
  }

}
