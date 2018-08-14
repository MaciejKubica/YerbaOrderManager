import { Component, OnInit, Directive, Output, EventEmitter, Input, SimpleChange } from "@angular/core"
import { DataService } from "../shared/dataService"
import { Router, ActivatedRoute } from "@angular/router"
import { YerbaAdderComponent } from "../yerbaadder/yerbaadder.component"
import { User } from "../shared/user";
import { Order, OrderItem } from "../shared/order";
import { Yerba } from "../shared/yerba";
import * as _ from "lodash";
import * as moment from 'moment';
import "eonasdan-bootstrap-datetimepicker";

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
    this.date = moment();
    this.currentUser = JSON.parse(localStorage.getItem("LoggedUser"));

    this.btnDisabled = this.isInRole;
  }

  public order: Order;

  public selectedUserMadeBy: User;
  public selectedYerba: Yerba;

  public selectedUserExecutedBy: User;

  public yerbas = [];
  public users: User[];

  public currentUser: User;
 
  public isPaid: boolean = false;

  public orderItems = [];

  public date: moment.Moment;

  public btnDisabled: boolean;

  compareFn(c1: User, c2: User): boolean {
    return c1 && c2 ? c1.id === c2.id : c1 === c2;
  }

  get totalcost(): number {
    return _.sum(_.map(this.orderItems, i => i.cost * i.quantity));
  }

  get totalQuantity(): number {
    return _.sum(_.map(this.orderItems, i => i.quantity));
  }

  ngOnInit(): void {
    this.loadUsers();
    this.loadYerbas();   
  }

  loadYerbas() {
    this.data.loadYerbas().subscribe(success => {
      if (success) {
        this.yerbas = this.data.yerbas;
        this.selectedYerba = this.yerbas[0];   
      } else {
        console.log("Not loaded");
      }
    });
  }

  loadUsers() {
    this.data.loadUsers().subscribe(success => {
      if (success) {
        this.users = this.data.users.filter(x => x.isDeleted == false);
        this.selectedUserMadeBy = this.users[0];
        this.selectedUserExecutedBy = this.users.find(x => x.orderTokenLocker);
         
      } else {
        console.log("Not loaded");
      }
    });
  }

  dateChange(event: any) {
    var item = event;

    this.date = moment(event, "DD/MM/YYYY");
  }

  dateClick(event: any) {
    var item = event;

    this.date = moment(event, "DD/MM/YYYY");
  }


  get isInRole(): boolean {
    return this.data.loggedUserRoles != null && this.data.loggedUserRoles.length > 0 && this.data.loggedUserRoles.some(this.isAdmin);
  }

  isAdmin(element, index, array) {
    return element === 'Administrator';
  }

  onCreate() {

    if (this.order.executedBy == null) {
      this.order.executedBy = this.selectedUserExecutedBy.id;
    }

    this.order.orderDate = this.date.toDate();

    this.order.madeBy = this.currentUser.id; 
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

  a2eOptions = {
    format: "DD/MM/YYYY"
  };

}
