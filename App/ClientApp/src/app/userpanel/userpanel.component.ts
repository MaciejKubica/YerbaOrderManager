import { Component, OnInit, Input, Directive } from "@angular/core";
import { DataService } from "../shared/dataService";
import { Router } from "@angular/router";
import { PasswordStrengthBar } from '../passwordStrengthBar';
import { User } from '../shared/user';
import { Order, OrderItem } from '../shared/order'

import { PaimentRequest } from '../shared/paimentrequest'
import * as _ from "lodash";
import { NG_VALIDATORS, AbstractControl } from "@angular/forms";
import { ChangePassword } from '../shared/changepassword';
import { CheckPassword } from "../shared/checkpassword";
import { Observable } from "rxjs/Observable";
import { TranslateService, LangChangeEvent } from "@ngx-translate/core";

function passwordUnMatcher(c: AbstractControl) {
  if (!c.get('password') || !c.get('newPassword')) return null;
  return c.get('password').value !== c.get('newPassword').value ? null : { 'nomatch': true };
}


@Directive({
  selector: '[password-matcher]',
  providers: [
    { provide: NG_VALIDATORS, multi: true, useValue: passwordUnMatcher }
  ]
})
export class PasswordWatcher {

}

@Component({
  selector: "app-userpanel",
  templateUrl: "./userpanel.component.html",
  styleUrls: ["./userpanel.component.css"]
})

export class UserPanelComponent implements OnInit {

  constructor(private data: DataService, private router: Router, private translate: TranslateService) {
    this.newPassword = "";
  }

  public user: User;
  public oldPasswordNotMatched: boolean;
  public changePasswordFails: boolean;

  public userOrderItems = [];
  public yerbas = [];
  public orders = [];
  public userOrders = [];
  public paiments: PaimentRequest[];
  public errorMessage: string;

   public paimentRequest = {
      orderitemid: 0,
      userid: 0
  };

  public barLabel: string;

  get subtotal(): number {
    return _.sum(_.map(this.userOrderItems, i => i.cost * i.quantity));
  }

  get sumquantity(): number {
    return _.sum(_.map(this.userOrderItems, i => i.quantity));
  }

  public newPassword: any;
  public retypeOldPassword: any;

  getOrderItems() {
    this.data.getLoggedUserOrderItems().subscribe(success => {
      if (success) {
        this.userOrderItems = this.data.ordersItems.filter(oi => oi.paid === false);
      }
    }, error => this.errorMessage = error);
  }

  getPaiments() {
    this.data.getPaimentsRequests().subscribe(success => {
      if (success) {
        this.paiments = this.data.paimentsRequests.filter(oi => oi.userId === this.user.id);
        this.getOrderItems();
      }
    }, error => this.errorMessage = error);
  }

  onGetYerba(id: number) {
    return this.yerbas.find(x => x.id === id);
  }

  onGetOrderDetail(id: number) {
    return this.orders.find(x => x.id === id);
  }

  onChangePassword() {

    let changePassword = new ChangePassword();

    changePassword.email = this.user.email;
    changePassword.oldPassword = this.retypeOldPassword;
    changePassword.newPassword = this.newPassword;

    this.data.changeUserPassword(changePassword).subscribe(success => {
      
      if (!success) {
        Error("Password not changed!");
      }
    });
  }

  onCheckPassword() {

    var checkPassword = new CheckPassword(this.user.id, this.retypeOldPassword);

    this.data.checkPassword(checkPassword).subscribe(() => {
      this.oldPasswordNotMatched = false;
    }, error => { this.oldPasswordNotMatched = true },
      () => {
        this.oldPasswordNotMatched = false;

      });
  }

  onChangeSettings() {
    this.data.editUserData(this.user).subscribe(success => {
      if (!success) {
        this.user = JSON.parse(localStorage.getItem("LoggedUser"));
      } else {
        localStorage.setItem("LoggedUser", JSON.stringify(this.user));
      }
    }, error => this.errorMessage = error);
  }

  loadYerbas() {
    this.data.loadYerbas().subscribe(success => {
      if (success) {
        this.yerbas = this.data.yerbas;
      } else {
        console.log("Not loaded");
      }
    }, error => this.errorMessage = error);
  }

  loadAllOrders() {
    this.data.loadAllOrders().subscribe(success => {
      if (success) {
        this.orders = this.data.orders;
        this.userOrders = this.data.orders.filter(x => x.madeBy === this.user.id && !x.isClosed);
      } else {
        console.log("Not loaded");
      }
    }, error => this.errorMessage = error);
  }

  closeOrder(order: Order) {
    this.data.closeOrder(order.id).subscribe(success => {
      if (!success) {
        console.log("Cannot close order");
      } else {
        this.loadAllOrders();
      }
    }, error => this.errorMessage = error);
  }

  onMakeToken(): void {
    this.data.makeNextOrderLocker().subscribe();
  }

  onCreateNewOrder(): void {
    this.router.navigate(["/createorder"]);
  }

  checkIsPaid(orderItem: OrderItem) {  
    return orderItem.paid === false && this.paiments.find(x => x.orderItemId === orderItem.id);
  }

  sendPaimentRequest(orderItem: any) {    
    this.paimentRequest.orderitemid = orderItem.id;
    this.paimentRequest.userid = this.user.id;

    this.data.createPaimentRequest(this.paimentRequest).subscribe(success => {
      if (success) {
        this.getPaiments();        
        this.getOrderItems();
      }
    }, error => this.errorMessage = error);
  }

  ngOnInit(): void {

    this.translate.get("GLOBAL.PASSWORDSTREANGTH").subscribe(
      data => {
        this.barLabel = data;
      });

    this.translate.onLangChange.subscribe((event: LangChangeEvent) => {
      //this.barLabel = this.translate.instant("GLOBAL.PASSWORDSTREANGTH");
      console.log(this.translate.instant("GLOBAL.PASSWORDSTREANGTH"));
      this.barLabel = this.translate.instant("GLOBAL.PASSWORDSTREANGTH");
    });

    this.user = JSON.parse(localStorage.getItem("LoggedUser"));
    this.getPaiments();
    this.loadYerbas();    
    this.loadAllOrders();    
  }
}
