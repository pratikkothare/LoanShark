import { Component, OnInit } from '@angular/core';
import { AdminService } from '../Services/admin.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {

  loans: any[] = [];
  pendingLoans: any[] = [];
  loading: boolean = false;
  errorMessage: string = '';
  showPopup: boolean = false;
  selectedLoan: any;
  actionType: string = '';
  adminComment: string = '';
  showVerifyPopup = false;
  verifyResult: any = null;
  loadingStep = 0;

  stats = {
    total: 0,
    approved: 0,
    pending: 0,
    rejected: 0
  };

  constructor(private adminService: AdminService,
    private http: HttpClient) { }

  ngOnInit(): void {
    this.fetchLoans();
  }

  fetchLoans(): void {
    this.loading = true;

    this.adminService.getLoans().subscribe({
      next: (res: any) => {
        this.loans = res || [];

        this.pendingLoans = this.loans.filter(
          (x: any) => x.status === 'Pending'
        );

        this.stats.total = this.loans.length;
        this.stats.approved = this.loans.filter(x => x.status === 'Approved').length;
        this.stats.pending = this.loans.filter(x => x.status === 'Pending').length;
        this.stats.rejected = this.loans.filter(x => x.status === 'Rejected').length;

        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load data';
        this.loading = false;
      }
    });
  }

  openPopup(loan: any, action: string) {
    this.selectedLoan = loan;
    this.actionType = action;
    this.adminComment = '';
    this.showPopup = true;
  }

  updateStatus(id: number, status: string) {
    this.adminService.updateLoanStatus(id, status).subscribe({
      next: () => {
        this.fetchLoans();
      },
      error: () => {
        alert('Update failed');
      }
    });
  }

  submitAction() {

    if (!this.adminComment.trim()) {
      alert("Please enter comment");
      return;
    }

    const id = this.selectedLoan.applicationId;
    const userId = this.selectedLoan.userId;

    this.adminService.updateLoanStatus(id, this.actionType).subscribe({
      next: () => {

        const message = `<b>Loan ID:</b> ${id} <br> 
                <b>Status:</b> ${this.actionType} <br> 
                                <b>Comment:</b> ${this.adminComment}`;

        this.adminService.sendNotification(userId, message).subscribe({
          error: () => {
            console.log("Notification failed");
          }
        });

        this.showPopup = false;
        this.fetchLoans();
      },
      error: () => {
        alert("Update failed");
      }
    });
  }

  download(appId: number) {
    window.open(`https://localhost:7156/api/Loan/download-documents/${appId}`, '_blank');
  }

  verifyDocs(appId: number, loan: any) {

    this.http.post(
      `https://localhost:7156/api/Loan/verify-docs/${appId}`, {}
    ).subscribe((res: any) => {

      loan.result = res;

    }, () => {
      alert("Verification failed");
    });

  }

  openVerifyPopup(loan: any) {
    this.showVerifyPopup = true;
    this.loadingStep = 0;
    this.verifyResult = null;
    this.selectedLoan = loan;

    // Step animation
    this.startFakeLoading();

    // Real API call
    this.http.post(
      `https://localhost:7156/api/Loan/verify-docs/${loan.applicationId}`, {}
    ).subscribe((res: any) => {
      this.verifyResult = res;
      this.loadingStep = 4;
    },
      () => {
        alert("Verification Failed");
      }
    );
  }

  startFakeLoading() {
    let step = 0;

    const interval = setInterval(() => {
      step++;
      if (step <= 3) {
        this.loadingStep = step;
      }

      if (step === 3) {
        clearInterval(interval);
      }
    }, 2500);
  }
}
