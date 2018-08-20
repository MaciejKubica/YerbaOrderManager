import { Component, OnInit } from "@angular/core"
import { DataService } from "../shared/dataService"
import { Router } from "@angular/router"
import { Order } from '../shared/order';
import { Role } from '../shared/role';

@Component({
  selector: "app-closedorders",
  templateUrl: "./closedorders.component.html",
  styleUrls: []
})
export class ClosedOrdersComponent implements OnInit {

  ngOnInit(): void { this.loadClosedOrders(); }

  constructor(private data: DataService, private router: Router) {

  }

  public orders: any[];
  public errorMessage: string;

  loadClosedOrders() {
    this.data.loadAllOrders().subscribe(success => {
      if (success) {
        this.orders = this.data.orders.filter(x =>  x.isClosed === true);
      } else {
        console.log("Not loaded");
      }
    }, error => this.errorMessage = error);
  }

  showOrder(order: Order) {
    this.router.navigate(["/showsummary"], { queryParams: { id: order.id } });
  }

}
