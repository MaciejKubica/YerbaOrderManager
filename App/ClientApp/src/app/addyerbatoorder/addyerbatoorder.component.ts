import { Component, OnInit } from "@angular/core"
import { DataService } from "../shared/dataService"
import { Router, ActivatedRoute } from "@angular/router"
import { User } from '../shared/user'
import { Yerba } from '../shared/yerba'
import { Order, OrderItem, CompactUser } from '../shared/order'

@Component({
  selector: "app-addyerbatoorder",
  templateUrl: "./addyerbatoorder.component.html",
  styleUrls: []
})
export class AddYerbaToOrderComponent implements OnInit {
  ngOnInit(): void {
    if (this.activatedRouter.queryParams) {
      this.activatedRouter.queryParams.subscribe(params => {
        if (params["id"]) {
          this.getOrderData(params["id"]);          
        }
      }, error => console.log(error));
    }

    this.loadUsers();
    this.loadYerbas();
    this.selectedUserMadeBy = this.users[0];
    this.selectedUserExecutedBy = this.users[0];
    this.selectedYerba = this.yerbas[0];
  }

  orderItems: any[];

  public selectedUserMadeBy: User;
  public selectedYerba: Yerba;

  public selectedUserExecutedBy: User;

  public yerbas = [];
  public users = [];

  currentOrder: Order;

  constructor(private data: DataService, private router: Router, private activatedRouter: ActivatedRoute) {
  }
  
  onNotify(message: OrderItem): void {

    console.log(message);

    let findOrderItem = this.orderItems.find(x => x.userId == message.userId && x.yerbaId == message.yerbaId);
    if (findOrderItem) {
      findOrderItem.quantity = findOrderItem.quantity + message.quantity;
      findOrderItem.orderId = this.currentOrder.id;
    } else {
      message.userDetails = new CompactUser();
      var userFound = this.users.find(x => x.id == message.userId);

      message.userDetails.email = userFound.email;
      message.userDetails.id = userFound.id;
      message.userDetails.name = userFound.name;

      message.orderId = this.currentOrder.id;

      this.orderItems.push(message);
    }

    this.onSubmitChanges();
  }

  getOrderData(id: any) {
    this.data.getOrderById(id).subscribe(success => {
      if (success) {
        this.currentOrder = this.data.orderData;
        this.orderItems = this.currentOrder.items;
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

  loadUsers() {
    this.data.loadUsers().subscribe(success => {
      if (success) {
        this.users = this.data.users.filter(x => x.isDeleted == false);
      } else {
        console.log("Not loaded");
      }
    });
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

  onSubmitChanges() {
    this.data.updateOrderItems(this.orderItems).subscribe(success =>
    {
      if (success) {
        this.router.navigate(["/home"]);
      }
    });
  }

}
