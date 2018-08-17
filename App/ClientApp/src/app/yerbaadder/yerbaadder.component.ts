import { Component, OnInit, EventEmitter, Output } from "@angular/core"
import { DataService } from "../shared/dataService"
import { Router } from "@angular/router"
import { User } from '../shared/user';
import { Order, OrderItem } from '../shared/order';
import { Yerba } from '../shared/yerba';

@Component({
  selector: "app-yerbaadder",
  templateUrl: "./yerbaadder.component.html",
  styleUrls: []
})

export class YerbaAdderComponent implements OnInit {
  
  constructor(private data: DataService, private router: Router) {
    this.loadYerbas();
    this.loadUsers();
  }  

  @Output() notify: EventEmitter<OrderItem> = new EventEmitter<OrderItem>();

  public selectedUser: any;
  public selectedYerba: any;
  public errorMessage: string;

  public quantity: number;
  public isPaid: boolean = false;

  public yerbas = [];
  public users = [];

  ngOnInit(): void {
    this.loadYerbas();
    this.loadUsers();
  }

  onGetYerba(id: number) {
    return this.yerbas.find(x => x.id === id);
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

  loadUsers() {
    this.data.loadUsers().subscribe(success => {
      if (success) {
        this.users = this.data.users.filter(x => x.isDeleted == false);
      } else {
        console.log("Not loaded");
      }
    }, error => this.errorMessage = error);
  }

  onClick() {
    let orderItem = new OrderItem();
    orderItem.id = 0;
    orderItem.userId = this.selectedUser;
    orderItem.yerbaId = this.selectedYerba;
    orderItem.cost = this.yerbas.find(y => y.id == this.selectedYerba).cost;
    orderItem.quantity = this.quantity;
    orderItem.paid = this.isPaid;
    
    orderItem.orderId = 0;    

    this.notify.emit(orderItem);
  }

  onChangeYerba(selectedYerba) {
    this.selectedYerba = selectedYerba;

  }

  onChangeUser(selectedUser) {
    this.selectedUser = selectedUser;
  }

  public get shouldAdd(): boolean {
    return this.quantity > 0 && this.selectedYerba.id > 0 && this.selectedUser.id > 0;
  }

}
