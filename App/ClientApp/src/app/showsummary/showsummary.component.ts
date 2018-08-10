import { Component, OnInit } from "@angular/core"
import { DataService } from "../shared/dataService"
import { ActivatedRoute, Router } from "@angular/router"
import { User } from '../shared/user'
import { Order, OrderItem } from '../shared/order';
import { Observable } from "rxjs/Observable";
import * as _ from "lodash";


@
  Component({
    selector: "app-showsummary",
    templateUrl: "./showsummary.component.html",
    styleUrls: []
  })


export class ShowSummaryComponent implements OnInit {

  public currentOrder: Order;
  public currentUser: User;
  public usertoPay: User;
  constructor(private data: DataService, private router: Router, private activatedRouter: ActivatedRoute) {
    this.currentOrder = new Order();
    this.currentUser = new User();
    this.usertoPay = new User();
  }

  users: any[];
  yerbas: any[];
  paiments: any[];
  calculatedBill: any;

  ngOnInit(): void {
    this.currentUser = JSON.parse(localStorage.getItem("LoggedUser"));
    this.loadUsers();
    this.loadYerbas();
    this.getPaiments();
    if (this.activatedRouter.queryParams) {
      this.activatedRouter.queryParams.subscribe(params => {
        if (params["id"]) {
          this.data.getOrderById(params["id"]).subscribe(success => {
            if (success) {
              this.currentOrder = this.data.orderData;
              this.calculateBillForPayUser();
              this.searchForPayUser();
            }
          });          
        }
      }, error => console.log(error));
    }
  }

  calculateBillForPayUser() {
    let itemsToPay = this.currentOrder.items.filter(x => x.userId == this.currentUser.id && x.paid === false);   
    this.calculatedBill =  _.sum(_.map(itemsToPay, i => i.quantity * i.cost));    
  }

  searchForPayUser() {
    this.usertoPay = this.users.find(x => x.id == this.currentOrder.executedBy);
  }

  getPaiments() {
    this.data.getPaimentsRequests().subscribe(success => {
      if (success) {
        this.paiments = this.data.paimentsRequests;
      }
    });
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

  loadYerbas() {
    this.data.loadYerbas().subscribe(success => {
      if (success) {
        this.yerbas = this.data.yerbas;
      } else {
        console.log("Not loaded");
      }
    });
  }

  confirmPaiment(orderItem: OrderItem) {

    var paimentRequest = {
      orderitemid: orderItem.id,
      userid: this.currentUser.id
    };

    this.data.confirmPaimentRequest(paimentRequest)
      .subscribe(success => {
        if (success) {          
          this.currentOrder.items.find(x => x.id === paimentRequest.orderitemid).paid = true;
          this.getPaiments();
        }
      });
  }

  checkIsPaid(orderItem: any) {
    return this.paiments.find(x => x.orderItemId === orderItem.id) != null;
  }

  paimentReceived(orderItem: any) {
    return !orderItem.paid && this.currentUser.id === this.currentOrder.madeBy && this.checkIsPaid(orderItem);
  }

  paimentDone(orderItem: any) {
    return orderItem.paid && this.currentUser.id === this.currentOrder.madeBy;
  }

  paimentNotReceived(orderItem: any) {
    return !orderItem.paid && this.currentUser.id === this.currentOrder.madeBy;
  }
  

  onGetYerba(id: number) {
    return this.yerbas.find(x => x.id === id);
  }

  getUserDetails(userId: any): User {
    return this.users.find(u => u.id === userId);
  }
}
