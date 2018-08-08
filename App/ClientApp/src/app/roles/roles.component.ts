import { Component, OnInit } from '@angular/core';
import { DataService } from "../shared/dataService"
import { User } from "../shared/user"
import { Router } from "@angular/router"
import { Order } from '../shared/order';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class RolesComponent implements OnInit {
  ngOnInit(): void {
    throw new Error("Method not implemented.");
  }
}
