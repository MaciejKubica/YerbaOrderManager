import { Component, OnInit } from "@angular/core"
import { DataService } from "../shared/dataService"
import { ActivatedRoute, Router } from "@angular/router"
import { User } from '../shared/user'
import { Order } from '../shared/order';
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
  calculatedBill: any;

  ngOnInit(): void {
    this.getUserData();
    this.loadUsers();
    this.loadYerbas();       
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
    let itemsToPay = this.currentOrder.items.filter(x => x.userId == this.currentUser.id && x.isPaid === false);   
    this.calculatedBill =  _.sum(_.map(itemsToPay, i => i.quantity * i.cost));    
  }

  searchForPayUser() {
    this.usertoPay = this.users.find(x => x.id == this.currentOrder.executedBy);
  }

  getUserData() {
    this.data.getLoggedUserData().subscribe(success => {
      if (success) {
        this.currentUser = this.data.userData;
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

  onGetYerba(id: number) {
    return this.yerbas.find(x => x.id === id);
  }

  getUserDetails(userId: any): User {
    return this.users.find(u => u.id === userId);
  }
}
