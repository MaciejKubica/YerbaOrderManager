import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { A2Edatetimepicker } from 'ng2-eonasdan-datetimepicker';
import { DateTimePickerDirective } from 'ng2-eonasdan-datetimepicker/dist/datetimepicker.directive'
import { AuthService } from "./shared/authService";



import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { UsersComponent } from './users/users.component';
import { YerbasComponent } from './yerbas/yerbas.component';
import { CreateEditUserComponent } from './create-edit-user/create-edit-user.component';
import { CreateEditYerbaComponent } from './create-edit-yerba/create-edit-yerba.component';
import { OrdersComponent } from './orders/orders.component';
import { CreateOrderComponent } from './createorder/createorder.component';
import { YerbaAdderComponent } from './yerbaadder/yerbaadder.component';
import { LoginComponent } from './login/login.component';
import { UserPanelComponent, PasswordWatcher } from './userpanel/userpanel.component';
import { PasswordStrengthBar } from './passwordStrengthBar';
import { EqualToValidatorDirective } from './shared/equalTo-validator-directive';
import { RegisterComponent } from './register/register.component';
import { ShowSummaryComponent } from './showsummary/showsummary.component';
import { AddYerbaToOrderComponent } from "./addyerbatoorder/addyerbatoorder.component";

import { DataService} from "./shared/dataService"
import { ErrorInterceptorProvider } from './shared/httperrorinterceptor';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    CreateEditUserComponent,
    UsersComponent,
    YerbasComponent,
    CreateEditYerbaComponent,
    OrdersComponent,
    CreateOrderComponent,
    YerbaAdderComponent,
    UserPanelComponent,
    PasswordStrengthBar,
    PasswordWatcher,
    EqualToValidatorDirective,
    LoginComponent,
    RegisterComponent,
    ShowSummaryComponent,
    AddYerbaToOrderComponent
  ],
  imports: [    
    A2Edatetimepicker,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,    
    FormsModule,    
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },   
      { path: 'users', component: UsersComponent, canActivate: [AuthService], canLoad: [AuthService] },
      { path: 'yerbas', component: YerbasComponent },
      { path: 'create-edit-user', component: CreateEditUserComponent, canActivate: [AuthService], canLoad: [AuthService] },
      { path: 'create-edit-yerba', component: CreateEditYerbaComponent, canActivate: [AuthService], canLoad: [AuthService] },
      { path: 'orders', component: OrdersComponent, canActivate: [AuthService], canLoad: [AuthService] },
      { path: 'createorder', component: CreateOrderComponent, canActivate: [AuthService], canLoad: [AuthService] },
      { path: 'userpanel', component: UserPanelComponent, canActivate: [AuthService], canLoad: [AuthService] },
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'showsummary', component: ShowSummaryComponent, canActivate: [AuthService], canLoad: [AuthService] },
      { path: 'addyerbatoorder', component: AddYerbaToOrderComponent, canActivate: [AuthService], canLoad: [AuthService] }
    ])    
  ],
  providers: [
    DataService,
    AuthService,
    ErrorInterceptorProvider
    ],
  bootstrap: [AppComponent]
})
export class AppModule { }
