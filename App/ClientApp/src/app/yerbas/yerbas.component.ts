import { Component, OnInit, AfterViewInit } from "@angular/core"
import { DataService} from "../shared/dataService"
import { Router } from "@angular/router"
import { Yerba } from '../shared/yerba';


@Component({
  selector: "app-yerbas",
  templateUrl: "./yerbas.component.html",
  styleUrls: []
})

export class YerbasComponent implements OnInit, AfterViewInit {

  ngAfterViewInit(): void {
    this.loadYerbas();
  }
  constructor(private data: DataService, private router: Router) {

  }

  public yerbas = [];

  ngOnInit(): void {

  }

  get hiddeButton() : boolean {
    return !this.data.loginRequired && this.isInAdministratorRole;
  }

  get isInAdministratorRole(): boolean {
    return this.data.loggedUserRoles != null && this.data.loggedUserRoles.find( x => x === "Administrator") != null;
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

  createYerba() {
    this.router.navigate(["/create-edit-yerba"]);
  }

  editYerba(yerba: Yerba) {
    console.log(yerba.id + " " + yerba.name + " " + yerba.cost);
    this.router.navigate(["/create-edit-yerba"], { queryParams: { id: yerba.id } });
  }

  deleteYerba(yerba: Yerba) {
    console.log(yerba.id + " " + yerba.name + " " + yerba.cost);
    this.data.deleteYerba(yerba).subscribe(success => {
      if (!success) {
        console.log("Not deleted!");
      } else {
        this.loadYerbas();
      }
    });
  }

}
