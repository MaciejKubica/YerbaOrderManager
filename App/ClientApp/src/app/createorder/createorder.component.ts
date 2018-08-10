import { Component, OnInit } from "@angular/core"
import { DataService } from "../shared/dataService"
import { Router, ActivatedRoute } from "@angular/router"
import { YerbaAdderComponent } from "../yerbaadder/yerbaadder.component"
import { User } from '../shared/user';
import { Order, OrderItem } from '../shared/order';
import { Yerba } from '../shared/yerba';
import * as _ from "lodash";

@Component({
  selector: "app-createorder",
  templateUrl: "./createorder.component.html",
  styleUrls: []
})


export class CreateOrderComponent implements OnInit {

  constructor(private data: DataService, private router: Router, private activatedRouter: ActivatedRoute) {
    this.order = new Order();
    this.orderItems = new Array<OrderItem>();
    this.order.created = new Date();
    this.order.isPaid = false;
    this.order.orderDate = new Date();
    this.order.id = 0;    
  }

  public order: Order;

  public selectedUserMadeBy: User;
  public selectedYerba: Yerba;

  public selectedUserExecutedBy: User;

  public yerbas = [];
  public users = [];
 
  public isPaid: boolean = false;

  public orderItems = [];

  get totalcost(): number {
    return _.sum(_.map(this.orderItems, i => i.cost * i.quantity));
  }

  get totalQuantity(): number {
    return _.sum(_.map(this.orderItems, i => i.quantity));
  }

  ngOnInit(): void {
    this.loadUsers();
    this.loadYerbas();
    this.selectedUserMadeBy = this.users[0];
    this.selectedUserExecutedBy = this.users[0];
    this.selectedYerba = this.yerbas[0];
  }

  loadYerbas() {
    this.data.loadYerbas().subscribe(success => {
      if (success) {
        this.yerbas = this.data.yerbas;
      } else {
        console.log("Not loaded");
      }
    });
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

  onCreate() {
    this.order.madeBy = this.data.userData.id; 
    this.order.totalCost = this.totalcost;
    this.order.totalQuantity = this.totalQuantity;

    if (this.orderItems.find(x => x.paid === false)) {
      this.order.isPaid = false;
    } else {
      this.order.isPaid = true;
    }

    this.order.items = this.orderItems;

    this.data.createOrder(this.order).subscribe(success => {
      if (!success) {
        console.log("Order not save");
      } else {
        this.router.navigate(["/orders"]);
      }
    });
  }

  onChangeExecutedBy(value: any) {
    this.order.executedBy = value;
  }

  onNotify(message: OrderItem): void {

    console.log(message);

    let findOrderItem = this.orderItems.find(x => x.userId === message.userId && x.yerbaId == message.yerbaId);
    if (findOrderItem) {
      findOrderItem.quantity = findOrderItem.quantity + message.quantity;
    } else {
      this.orderItems.push(message);
    }
  }

  onGetYerba(id: number) {
    return this.yerbas.find(x => x.id == id);
  }

  deleteOrderItem(oi: OrderItem) {
    let indexToRemove = this.orderItems.indexOf(oi);

    if (indexToRemove > -1) {
      this.orderItems.splice(indexToRemove, 1);
    }

  }

  options = {
    format: "DD-MM-YYYY"
  };

}
