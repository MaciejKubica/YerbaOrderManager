import { Component, OnInit } from "@angular/core"
import { DataService } from "../shared/dataService"
import { Router, ActivatedRoute } from "@angular/router"
import { Yerba } from '../shared/yerba';

@Component({
  selector: "app-create-edit-yerba",
  templateUrl: "./create-edit-yerba.component.html",
  styleUrls: []
})
export class CreateEditYerbaComponent implements OnInit {

  constructor(private data: DataService, private router: Router, private activatedRouter: ActivatedRoute) {

    this.yerba = new Yerba();
  }

  public openForEdit: boolean = false;

  public yerba: Yerba;

  ngOnInit(): void {
    if (this.activatedRouter.queryParams) {
      this.activatedRouter.queryParams.subscribe(params => {
        if (params["id"]) {
          this.getYerbaData(params["id"]);
          this.openForEdit = true;
        }       

      }, error => console.log(error));
    }
  }

  getYerbaData(id: any) {
    this.data.getYerbaData(id).subscribe(success => {
      if (success) {
        this.yerba = this.data.yerbaData;
      }
    });
  }

  onCreate() {
    if (this.openForEdit) {
      this.data.updateYerba(this.yerba).subscribe(success => {
        if (!success) {
          console.log("Not edited!");
        } else {
          this.router.navigate(["/yerbas"]);
        }
      });
    } else {
      this.data.createYerba(this.yerba).subscribe(success => {
        if (!success) {
          console.log("Not created!");
        } else {
          this.router.navigate(["/yerbas"]);
        }
      });
    }    
  }

}

