import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomepageComponent } from './homepage/homepage.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { LoanproductsComponent } from './loanproducts/loanproducts.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { AdminUsersComponent } from './admin-users/admin-users.component';
import { CheckEligibiltyComponent } from './check-eligibilty/check-eligibilty.component';
import { UserprofileComponent } from './userprofile/userprofile.component';
import { EmiCalculatorComponent } from './emi-calculator/emi-calculator.component';
import { ApplyLoanComponent } from './apply-loan/apply-loan.component';
import { ShownotificationComponent } from './shownotification/shownotification.component';
import { AdminAuthGuard } from './guards/admin-auth.guard';
import { UserAuthGuard } from './guards/user-auth.guard';
import { AdminProductsComponent } from './admin-products/admin-products.component';
import { AdminAnalyticsComponent } from './admin-analytics/admin-analytics.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent ,canActivate:[UserAuthGuard]},
  { path: 'admin-dashboard', component: AdminDashboardComponent ,canActivate:[AdminAuthGuard] },
  { path: 'loanproducts', component: LoanproductsComponent, canActivate: [UserAuthGuard] },
  { path: 'admin-users', component: AdminUsersComponent, canActivate: [AdminAuthGuard] },
  { path: 'reset-password', component: ResetPasswordComponent },
  { path: 'check-eligibilty/:id', component: CheckEligibiltyComponent, canActivate: [UserAuthGuard] },
  { path: 'profile', component: UserprofileComponent, canActivate: [UserAuthGuard] },
  { path: 'emi-calculator', component: EmiCalculatorComponent, canActivate: [UserAuthGuard] },
  { path: 'apply-loan', component: ApplyLoanComponent, canActivate: [UserAuthGuard] },
  { path: 'notification', component: ShownotificationComponent, canActivate: [UserAuthGuard] },
  { path: 'admin-products', component: AdminProductsComponent, canActivate: [AdminAuthGuard] },
  { path: 'admin-analytics', component: AdminAnalyticsComponent, canActivate: [AdminAuthGuard] },
  { path: '', component: HomepageComponent },
  { path: '**', component: HomepageComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
