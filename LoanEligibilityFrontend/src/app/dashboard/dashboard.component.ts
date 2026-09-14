import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Router ,RouterLink,RouterModule} from '@angular/router';
import { LoanDataService } from '../Services/loan-data.service';
import { AuthService } from '../Services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone:true,
  templateUrl: './dashboard.component.html',
  imports: [RouterLink, CommonModule],
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent {
  products: any[] = [];
  applications: any[] = [];
  userId: number = 0;
  userName: string = '';
  firstName: string = '';
  total = 0;
  approved = 0;
  pending = 0;
  rejected = 0;
  greeting: string = '';
  openMenuId: number | null = null;
  isProcessing = false;
  showPopup= false;
  showConfirm= false;
  popupMessage = '';
  selectedAppId: number = 0;
  constructor(private router: Router, private http: HttpClient, private loanservice: AuthService) { }

  ngOnInit() {
    this.getProducts();
    this.loadUser();
    this.setGreeting();
  }

  getProducts() {
    this.http.get<any>('https://localhost:7156/api/Loan/products').subscribe(
      res => {
        if (res.success) {

         
          this.products = res.data.map((p: any) => ({
            ...p,
            appl: this.generateCount(p.productName)
          }));

        }
      }
    );
  }

  loadUser() {
    const data = localStorage.getItem('user');

    if (data) {
      const user = JSON.parse(data);
      this.userId = user.userId;
      this.userName = user.fullName || '';
      this.firstName = this.userName.split(' ')[0];
      this.getApplications();
    }
  }

  getApplications() {
    this.http.get<any>(`https://localhost:7156/api/Loan/getloans/${this.userId}`)
      .subscribe(res => {
        if (res.success) {
          this.applications = res.data;
          this.total = this.applications.length;

          this.approved = this.applications.filter(a => a.status === 'Approved').length;
          this.pending = this.applications.filter(a => a.status === 'Pending').length;
          this.rejected = this.applications.filter(a => a.status === 'Rejected').length;
        }
      });
  }

  setGreeting() {
    const hour = new Date().getHours();

    if (hour < 12) {
      this.greeting = 'Good Morning';

    }
    else if (hour < 17) {
      this.greeting = 'Good Afternoon';
    }
    else {
      this.greeting = 'Good Evening';
    }
  }

  getLoanType(productId: number) {
    switch (productId) {
      case 1: return 'Home Loan';
      case 2: return 'Car Loan';
      case 3: return 'Personal Loan';
      case 4: return 'Education Loan';
      case 5: return 'Gold Loan'
      default: return 'Unknown';
    }
  }

  download(appId: number) {
    window.open(`https://localhost:7156/api/Loan/download-documents/${appId}`, '_blank');
  }

  toggleMenu(id: number) {
    this.openMenuId = this.openMenuId === id ? null : id;
  }


  confirmWithdraw(app: any) {

    if (app.status === 'Approved') {
      this.popupMessage = 'Approved loan cannot be withdrawn';
      this.showPopup = true;
      return;
    }

    if (app.status === 'Rejected') {
      this.popupMessage = 'Rejected application cannot be withdrawn';
      this.showPopup = true;
      return;
    }

    if (app.status === 'Withdrawn') {
      this.popupMessage = 'Application already withdrawn';
      this.showPopup = true;
      return;
    }

    this.selectedAppId = app.applicationId;
    this.showConfirm = true;

  }

  withdrawNow() {

    this.showConfirm = false;
    this.isProcessing = true;

    this.loanservice.withdrawApplication(this.selectedAppId)
      .subscribe({

        next: (res) => {

          setTimeout(() => {

            this.isProcessing = false;

            this.popupMessage = 'Application withdrawn successfully';
            this.showPopup = true;

          }, 2000);

        },

        error: () => {
          this.isProcessing = false;
          this.popupMessage = 'Withdraw failed';
          this.showPopup = true;
        }

      });
  }

  closePopup() {
    this.showPopup = false;
    if (this.popupMessage === 'Application withdrawn successfully') {
      window.location.reload();
    }
  }

  generateCount(productName: string): string {

    const counts: any = {
      'Home Loan': '10.8K',
      'Car Loan': '7.2K',
      'Personal Loan': '15.4K',
      'Education Loan': '5.6K'
    };

    return counts[productName] || '3.1K';
  }
}
