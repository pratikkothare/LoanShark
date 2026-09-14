import { Component, OnInit } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { LoanDataService } from '../Services/loan-data.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { Router } from '@angular/router';

@Component({
  selector: 'app-apply-loan',
  templateUrl: './apply-loan.component.html',
  styleUrls: ['./apply-loan.component.css']
})
export class ApplyLoanComponent implements OnInit {

  loanAmount: number = 0;
  tenureMonths: number = 0;
  income: number = 0;
  creditScore: number = 0;
  showSuccessModal = false;
  isProcessing= false;
  showSuccessTick = false;
  applicationStatus = "Pending Verification";
  result: string = "";
  responseData: any;
  showError = false;

  
  applicationId: number | null = null;

  files: any = {
    aadhaar: null,
    pan: null,
    salary: null,
    bank: null
  };

  filesErrors: any = {
    aadhar: '',
    pan: '',
    salary: '',
    bank: ''
  };

 uploading:any={
 aadhaar:false,
 pan:false,
 salary:false,
 bank:false
};

uploaded:any={
 aadhaar:false,
 pan:false,
 salary:false,
 bank:false
};

  filePreviews: { [key: string]: SafeUrl | null } = {
    aadhaar: null,
    pan: null,
    salary: null,
    bank: null
  };

  fileTypes: any = {
    aadhaar: { isImage: false, isPdf: false },
    pan: { isImage: false, isPdf: false },
    salary: { isImage: false, isPdf: false },
    bank: { isImage: false, isPdf: false }
  };

  constructor(
    private auth: AuthService,
    public loanService: LoanDataService,
    private sanitizer: DomSanitizer,
    private router: Router
  ) { }

  ngOnInit() {
    this.loanAmount = this.loanService.sharedLoanAmount;
    this.tenureMonths = this.loanService.tenureMonths;
    this.income = this.loanService.sharedIncome;
    this.creditScore = this.loanService.creditScore;
  }

  onFileSelected(event: any, type: string) {

    const file = event.target.files[0];
    if (!file) return;

    const allowedTypes = [
      'image/jpeg',
      'image/jpg',
      'image/png',
      'application/pdf'
    ];

    const maxSize = 1 * 1024 * 1024;

    if (!allowedTypes.includes(file.type)) {
      alert('Only JPG, PNG and PDF files are allowed');
      event.target.value = '';
      return;
    }

   
    if (file.size > maxSize) {
      alert('File size must be less than 1 MB');
      event.target.value = '';
      return;
    }

    this.uploading[type] = true;
    this.uploaded[type] = false;

    setTimeout(() => {
      this.uploading[type] = false;
      this.uploaded[type] = true;
    }, 2000);

    this.files[type] = file;

    const url = URL.createObjectURL(file);

    this.filePreviews[type] =
      this.sanitizer.bypassSecurityTrustUrl(url);

    const fileType = file.type;

    this.fileTypes[type] = {
      isImage: fileType.startsWith('image/'),
      isPdf: fileType === 'application/pdf'
    };

  }

  // APPLY LOAN
  applyLoan() {

    const user = JSON.parse(localStorage.getItem("user") || '{}');

    const formData = new FormData();

    formData.append('userId', user.userId.toString());
    formData.append('productId', this.loanService.productId.toString());
    formData.append('loanAmount', this.loanAmount.toString());
    formData.append('tenureMonths', this.tenureMonths.toString());
    formData.append('income', this.income.toString());
    formData.append('creditScore', this.creditScore.toString());

    if (this.files.aadhaar) {
      formData.append('aadhaar', this.files.aadhaar);
    }

    if (this.files.pan) {
      formData.append('pan', this.files.pan);
    }

    if (this.files.salary) {
      formData.append('salary', this.files.salary);
    }

    if (this.files.bank) {
      formData.append('bank', this.files.bank);
    }

    if (!this.allDocsUploaded()) {
      this.showError = true;
      return;
    }
    this.showError = false;
    // Object.keys(this.files).forEach(key => {
    //   if (this.files[key]) {
    //     formData.append('files', this.files[key]);
    //   }
    // });

    this.auth.applyLoan(formData).subscribe({
      next: (res: any) => {
        console.log("API RESPONSE:", res);

        this.responseData = res;
        this.applicationId = res.applicationId;

        if (res.success) {
          this.showSuccessModal = true;
          this.isProcessing = true;

          setTimeout(() => {
            this.isProcessing = false;
            this.showSuccessTick = true;
          }, 3000);
          //this.result = "Loan Applied Successfully. Application Id: " + res.applicationId;
        } else {
          this.result = "Loan Application Failed";
        }
      },
      error: () => {
        this.result = "Something went wrong";
      }
    });
  }

  // DOWNLOAD DOCUMENTS
  downloadDocuments() {

    console.log("DOWNLOAD CLICK:", this.applicationId);

    if (!this.applicationId) {
      alert("Apply loan first");
      return;
    }

    const url = `https://localhost:7156/api/loan/download-documents/${this.applicationId}`;

    // ✅ FORCE DOWNLOAD
    const link = document.createElement('a');
    link.href = url;
    link.target = '_blank';
    link.click();
  }

  goDashboard() {
    this.router.navigate(['/dashboard']);
  }

  downloadDoc() {
    this.downloadDocuments();
  }

  allDocsUploaded(): boolean {
    return this.uploaded.aadhaar &&
      this.uploaded.pan &&
      this.uploaded.salary &&
      this.uploaded.bank;
  }
}
