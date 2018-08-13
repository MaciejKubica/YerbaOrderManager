import { Component, Inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import "rxjs/add/operator/map";
import { User } from './user';
import { Yerba } from './yerba';
import { Order, OrderItem } from './order';
import { ChangePassword} from './changepassword';
import { Observable } from 'rxjs/Observable';
import { PaimentRequest } from './paimentrequest';


@Injectable()
export class DataService {

  constructor(private http: HttpClient) {
    this.token = "";
    this.tokenExpiration = new Date();
  }

  public users = [];
  
  public loadUsers() {
    let headers = new HttpHeaders()      
      .set("Authorization", "Bearer " + this.token);

    return this.http.get("/api/ordermanager/getusers", { headers })
      .map((data: any[]) => {
        this.users = data;
        return true;
      }, error => console.error(error));
  }

  public createUser(user: User) {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
      .set("Authorization", "Bearer " + this.token);

    var jsonString = JSON.stringify(user);
    console.log(jsonString);
    return this.http.post("/api/ordermanager/createuser", jsonString, { headers });
  }

  public changeUserPassword(changePassword: ChangePassword) {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
      .set("Authorization", "Bearer " + this.token);

    var jsonString = JSON.stringify(changePassword);
    console.log(jsonString);
    return this.http.post("/api/ordermanager/changeuserpassword", jsonString, { headers });
  }

  public checkPassword(changePassword: any) {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
      .set("Authorization", "Bearer " + this.token);      

    var jsonString = JSON.stringify(changePassword);
    
    return this.http.post("/api/ordermanager/checkuserpassword", jsonString, { headers });
  }

  public updateUser(user: User) {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
      .set("Authorization", "Bearer " + this.token);

    var jsonString = JSON.stringify(user);
    console.log(jsonString);
    return this.http.post("/api/ordermanager/edituser", jsonString, { headers });
  }

  public deleteUser(user: any) {

    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
      .set("Authorization", "Bearer " + this.token);

    var jsonString = JSON.stringify(user.email);

    return this.http.post("/api/ordermanager/deleteuser", jsonString, { headers });
  }

  public yerbas = [];

  public loadYerbas() {
    return this.http.get("/api/ordermanager/getyerbas")
      .map((data: any[]) => {
        this.yerbas = data;
        return true;
      }, error => console.error(error));
  }

  public createYerba(yerba: Yerba) {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
      .set("Authorization","Bearer " + this.token);

    var jsonString = JSON.stringify(yerba);
    console.log(jsonString);
    return this.http.post("/api/ordermanager/createyerba", jsonString, { headers });
  }

  public updateYerba(yerba: Yerba) {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
      .set("Authorization", "Bearer " + this.token);

    var jsonString = JSON.stringify(yerba);
    console.log(jsonString);
    return this.http.post("/api/ordermanager/edityerba", jsonString, { headers });
  }

  public deleteYerba(yerba: any) {

    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
      .set("Authorization", "Bearer " + this.token);

    var jsonString = JSON.stringify(yerba.id);

    return this.http.post("/api/ordermanager/deleteyerba", jsonString, { headers });
  }

  public orders = [];

  public loadAllOrders() {
    return this.http.get("/api/ordermanager/getallorders")
      .map((data: any[]) => {
        this.orders = data;
        return true;
      }, error => console.error(error));
  }

  public createOrder(order: Order) {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
      .set("Authorization", "Bearer " + this.token);

    var jsonString = JSON.stringify(order);
    console.log(jsonString);
    return this.http.post("/api/ordermanager/createorder", jsonString, { headers });
  }

  public updateOrderItems(orderItems: OrderItem[]) {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
      .set("Authorization", "Bearer " + this.token);

    var jsonString = JSON.stringify(orderItems);  
    return this.http.post("/api/ordermanager/updateorderitems", jsonString, { headers });
  }


  public deleteOrder(id: number) {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
      .set("Authorization", "Bearer " + this.token);

    var jsonString = JSON.stringify(id);

    return this.http.post("/api/ordermanager/deleteorder", jsonString, { headers });
  }

  public getOrderById(id: number) {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
      .set("Authorization", "Bearer " + this.token)
      .set("id", id.toString());    
      
    return this.http.get("/api/ordermanager/getorderbyid", { headers }).map((data: Order) => {
      this.orderData = data;
      return true;
    }, error => console.error(error));
  }

  public closeOrder(id: number) {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
      .set("Authorization", "Bearer " + this.token);

    var jsonString = JSON.stringify(id);

    return this.http.post("/api/ordermanager/closeorder", jsonString, { headers });
  }

  public ordersItems = [];

  public getUserOrderItems(email: any) {

    let headers = new HttpHeaders()
      .set("email", email);
   
    return this.http.get("/api/ordermanager/getuserorderitems", { headers })
      .map((data: any[]) => {
        this.ordersItems = data;
        return true;
      }, error => console.error(error));
  }

  public userData : any;

  public getUserData(email: any) {
    
    let params = new HttpParams().set('email', email);

    let headers = new HttpHeaders()      
      .set("Authorization", "Bearer " + this.token);
        
    return this.http.get("/api/ordermanager/getuserbyemail", { headers, params })
      .map((data: any[]) => {
      this.userData = data;
      return true;
    }, error => console.error(error));
  }

  public getLoggedUserData() {

    let headers = new HttpHeaders()
      .set("Authorization", "Bearer " + this.token);

    return this.http.get("/api/ordermanager/getloggeduserdata", { headers })
      .map((data: any[]) => {
       localStorage.setItem("LoggedUser", JSON.stringify(data));

        return true;
      }, error => console.error(error));
  }

  public getUserDetails(userId: any) {
    let params = new HttpParams().set('id', userId);

    return this.http.get("/api/ordermanager/getuserbyid", { params });
  }

  public getLoggedUserOrderItems() {

    let headers = new HttpHeaders()
      .set("Authorization", "Bearer " + this.token);

    return this.http.get("/api/ordermanager/getloggeduserorderitems", { headers })
      .map((data: any[]) => {
        this.ordersItems = data;
        return true;
      }, error => console.error(error));
  }

  public makeNextOrderLocker() {

    let headers = new HttpHeaders()
      .set("Authorization", "Bearer " + this.token);

    return this.http.get("/api/ordermanager/makenextorderlocker", { headers });
  }

  public editUserData(user: User) {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
      .set("Authorization", "Bearer " + this.token);

    var jsonString = JSON.stringify(user);
    console.log(jsonString);
    return this.http.post("/api/ordermanager/changeuserdata", jsonString, { headers })
      .map((data: any) => {
        return true;
        });
  }

  public registerUser(user: User): Observable<boolean> {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json");

    var jsonString = JSON.stringify(user);
    console.log(jsonString);
    return this.http.post("/account/registeruser", jsonString, { headers })
      .map((data: any) => {
        this.loggedUserRoles = data.roles;
      this.token = data.token;
      this.tokenExpiration = data.expiration;
      return true;
    });;
  }

  public yerbaData: any;

  public getYerbaData(id: any) {

    let params = new HttpParams().set('id', id);

    return this.http.get("/api/ordermanager/getyerbabyid", { params })
      .map((data: any[]) => {
        this.yerbaData = data;
        return true;
      }, error => console.error(error));
  }

  token: any;
  tokenExpiration: any;
  loggedUserRoles: any[];
  loggedUserEmail: string;

  login(creds) : Observable<boolean> {
    return this.http
      .post("/account/createtoken", creds)
      .map((data: any) => {
        this.loggedUserRoles = data.roles;
        this.token = data.token;
        this.tokenExpiration = data.expiration;
        this.loggedUserEmail = creds.username;
      return true;
    });
  }

  logoff() {
    this.token = "";
    this.tokenExpiration = new Date();
    this.loggedUserRoles = null;
    localStorage.clear();
  }

  isInRole(role: string): boolean {
    return this.loggedUserRoles != null && this.loggedUserRoles.length > 0 && this.loggedUserRoles.indexOf(x => x === role) > 0;
  }

  get loginRequired(): boolean {
    return this.token.length == 0 || this.tokenExpiration > new Date();
  }

  orderData: Order;

  rolesInSystem: any[];

  public getRoles() {

    let headers = new HttpHeaders()
      .set("Authorization", "Bearer " + this.token);

    return this.http.get("/api/ordermanager/getroles", { headers })
      .map((data: any[]) => {
        this.rolesInSystem = data;
        return true;
      }, error => console.error(error));
  }

  //Paiments

  paimentsRequests: PaimentRequest[];

  public getPaimentsRequests() {

    let headers = new HttpHeaders()
      .set("Authorization", "Bearer " + this.token);

    return this.http.get("/api/ordermanager/getpaimentsrequests", { headers })
      .map((data: any[]) => {
        this.paimentsRequests = data;
        return true;
      }, error => console.error(error));
  }

  public confirmPaimentRequest(paimentRequest: any) {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
    .set("Authorization", "Bearer " + this.token);

    var jsonString = JSON.stringify(paimentRequest);
    console.log(jsonString);
    return this.http.post("/api/ordermanager/confirmpaimentrequest", jsonString, { headers }).map((response: any) => { return true; });     
  }

  public createPaimentRequest(paimentRequest: any): Observable<boolean> {
    let headers = new HttpHeaders()
      .set("Content-Type", "application/json")
    .set("Authorization", "Bearer " + this.token);

    var jsonString = JSON.stringify(paimentRequest);
    console.log(jsonString);
    return this.http.post("/api/ordermanager/createpaimentrequest", jsonString, { headers })
      .map((response: any) => { return true; });     
  }

}
