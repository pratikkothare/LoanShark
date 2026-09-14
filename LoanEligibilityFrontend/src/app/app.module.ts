import { NgModule } from '@angular/core';

import { BrowserModule } from '@angular/platform-browser';

import { FormsModule } from '@angular/forms';

import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';

import { HomepageComponent } from './homepage/homepage.component';

import { LoginComponent } from './login/login.component';

import { RegisterComponent } from './register/register.component';

import { NavbarComponent } from './navbar/navbar.component';

import { AppRoutingModule } from './app-routing.module';

import { CommonModule } from '@angular/common';

import { UserprofileComponent } from './userprofile/userprofile.component';

import { CheckEligibiltyComponent } from './check-eligibilty/check-eligibilty.component';

import { RouterModule } from '@angular/router';

import { AdminSidebarComponent } from './admin-sidebar/admin-sidebar.component';

import { ResetPasswordComponent } from './reset-password/reset-password.component';

import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';

import { AdminUsersComponent } from './admin-users/admin-users.component';

import { EmiCalculatorComponent } from './emi-calculator/emi-calculator.component';
import { ApplyLoanComponent } from './apply-loan/apply-loan.component';
import { ShownotificationComponent } from './shownotification/shownotification.component';
import { AdminProductsComponent } from './admin-products/admin-products.component';
import { AdminAnalyticsComponent } from './admin-analytics/admin-analytics.component';
import { ChatbotComponent } from './chatbot/chatbot.component';

@NgModule({

  declarations: [

    AppComponent,

    HomepageComponent,

    LoginComponent,

    RegisterComponent,

    NavbarComponent,

    UserprofileComponent,

    CheckEligibiltyComponent,

    AdminSidebarComponent,

    AdminDashboardComponent,

    AdminUsersComponent,

    ResetPasswordComponent,

    EmiCalculatorComponent,
    ApplyLoanComponent,
    ShownotificationComponent,
    AdminProductsComponent,
    AdminAnalyticsComponent,
    ChatbotComponent


  ],

  imports: [

    BrowserModule,

    FormsModule,

    HttpClientModule,

    AppRoutingModule,

    CommonModule,

    RouterModule

  ],
  providers: [],
  bootstrap: [AppComponent]

})

export class AppModule { }




