import { Component, OnInit } from "@angular/core"
import { DataService } from "../shared/dataService"
import { Router } from "@angular/router"
import { Order } from '../shared/order';

@Component({
  selector: "app-orders",
  templateUrl: "./orders.component.html",
  styleUrls: []
})


export class OrdersComponent implements OnInit {

  constructor(private data: DataService, private router: Router) {

  }

  public orders = [];

  ngOnInit(): void {
    this.loadAllOrders();
  }

  get hiddeButton(): boolean {
    return !this.data.loginRequired && this.isInAdministratorRole;
  }

  get isInAdministratorRole(): boolean {
    return this.data.loggedUserRoles != null && this.data.loggedUserRoles.find(x => x === "Administrator") != null;
  }

  loadAllOrders() {
    this.data.loadAllOrders().subscribe(success => {
      if (success) {
        this.orders = this.data.orders;
      } else {
        console.log("Not loaded");
      }
    });
  }

  createOrder() {
    this.router.navigate(["/createorder"]);

  }

  closeOrder(order: Order) {
    this.data.closeOrder(order.id).subscribe(success => {
      if (success)
        this.loadAllOrders();
    });
  }

  deleteOrder(order: Order) {
    this.data.deleteOrder(order.id).subscribe(success => {
      if (success) {
        this.loadAllOrders();
      }
    });
  }

}
