import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LoanDataService } from '../Services/loan-data.service';

@Component({
  selector: 'app-check-eligibilty',
  templateUrl: './check-eligibilty.component.html',
  styleUrls: ['./check-eligibilty.component.css']
})
export class CheckEligibiltyComponent implements OnInit {

  productId: number = 0;

  income: number = 0;
  creditScore: number = 0;
  propertyValue: number = 0;
  coApplicantIncome: number = 0;
  carPrice: number = 0;
  downPayment: number = 0;
  purpose: string = "";
  yearsInCity: number = 0;
  result: string = "";
  course: string = "";
  university: string = "";
  eduLoanAmount: number = 0;
  goldWeight: number = 0;
  goldPurity: number = 0;
  eligible: boolean | null = null;
  submitted: boolean = false;
  loanAmount: number | null = null;
  steps : any = {
    cibil: 'pending',
    income: 'pending',
    eligibility: 'pending'
  };

  isChecking: boolean = false;
  allDone: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private loanService: LoanDataService
  ) { }

  ngOnInit() {
    this.productId = Number(this.route.snapshot.paramMap.get('id'));
    this.loanService.productId = this.productId;
  }


  validateAndCheck() {

    this.submitted = true;

    if (this.income <= 0 || this.creditScore <= 0) {
      this.fail("Enter valid income & credit score");
      return;
    }

    
    if (this.productId == 1 && this.propertyValue <= 0) {
      this.fail("Enter valid property value");
      return;
    }

    // CAR LOAN
    if (this.productId == 2) {

      if (this.carPrice <= 0 || this.downPayment < 0) {
        this.fail("Enter valid car details");
        return;
      }

      if (this.downPayment > this.carPrice) {
        this.fail("Down payment cannot exceed car price");
        return;
      }
    }

    // PERSONAL LOAN
    if (this.productId == 3) {
      if (!this.loanAmount || this.loanAmount <= 0) {
        this.fail("Enter valid Loan Amount");
        return;
      }
      if (!this.purpose || this.yearsInCity < 1) {
        this.fail("Fill all personal loan details properly");
        return;
      }
    }

    // EDUCATION LOAN
    if (this.productId == 4) {

      if (!this.course || !this.university) {
        this.fail("Enter course & college details");
        return;
      }

      if (!this.eduLoanAmount || this.eduLoanAmount < 50000) {
        this.fail("Minimum education loan ₹50,000");
        return;
      }
    }

    // GOLD LOAN
    if (this.productId == 5) {

      if (this.goldWeight <= 0 || this.goldPurity <= 0) {
        this.fail("Enter valid gold details");
        return;
      }

      if (this.goldPurity < 50 || this.goldPurity > 100) {
        this.fail("Gold purity must be between 50% - 100%");
        return;
      }
    }

    this.checkEligibilty();
  }

  // MAIN FLOW
  checkEligibilty() {

    this.resetSteps();
    this.isChecking = true;

    //CIBIL
    setTimeout(() => {

      if (this.creditScore < 650) {
        this.steps.cibil = 'failed';
        this.fail("Credit Score must be above 650");
        return;
      }
      this.loanService.creditScore = this.creditScore;
      this.steps.cibil = 'done';

      setTimeout(() => {

        if (this.income < 25000) {
          this.steps.income = 'failed';
          this.fail("Income must be above ₹25,000");
          return;
        }

        this.loanService.sharedIncome = this.income;
        this.steps.income = 'done';

        //PRODUCT LOGIC
        setTimeout(() => {
          this.handleProductLogic();
        }, 1000);

      }, 1000);

    }, 1000);
  }


  handleProductLogic() {

    // HOME
    if (this.productId == 1) {

      if (this.propertyValue <= 0) {
        this.steps.eligibility = 'failed';
        this.fail("Invalid Property Value");
        return;
      }
      this.loanService.sharedLoanAmount = this.propertyValue;
      this.loanService.productId = this.productId;
      this.success("Eligible for Home Loan");
    }

    // CAR
    else if (this.productId == 2) {

      if (this.carPrice <= 0) {
        this.steps.eligibility = 'failed';
        this.fail("Invalid Car Price");
        return;
      }

      if (this.downPayment < 0) {
        this.steps.eligibility = 'failed';
        this.fail("Invalid Down Payment");
        return;
      }
      this.loanService.productId = this.productId;
      this.loanService.sharedLoanAmount = this.carPrice - this.downPayment;
      this.success("Eligible for Car Loan");
    }

    // PERSONAL
    else if (this.productId == 3) {

      if (this.yearsInCity < 1) {
        this.steps.eligibility = 'failed';
        this.fail("Minimum 1 year stay required");
        return;
      }
      this.loanService.productId = this.productId;
      this.loanService.sharedLoanAmount = this.loanAmount || 0;
      this.success("Eligible for Personal Loan");
    }

    // Education
    else if (this.productId == 4) {

      if (!this.eduLoanAmount || this.eduLoanAmount < 50000) {
        this.steps.eligibility = 'failed';
        this.fail("Minimum loan amount ₹50,000");
        return;
      }

      if (!this.course || !this.university) {
        this.steps.eligibility = 'failed';
        this.fail("Course & University required");
        return;
      }

      if (this.coApplicantIncome < 500000) {
        this.steps.eligibility = 'failed';
        this.fail("Co-applicant income must be greater than 5,00,000");
        return;
      }

      if (this.creditScore < 650) {
        this.steps.eligibility = 'failed';
        this.fail("Low credit score");
        return;
      }

      // Max eligibility
      const maxLoan = this.coApplicantIncome * 10;

      if (this.eduLoanAmount > maxLoan) {
        this.steps.eligibility = 'failed';
        this.fail(`Max eligible loan ₹${maxLoan}`);
        return;
      }

      this.loanService.productId = this.productId;
      this.loanService.sharedLoanAmount = this.eduLoanAmount;

      this.success("Eligible for Education Loan");
    }

    //Gold
    else if (this.productId == 5) {

      if (this.goldWeight <= 0) {
        this.steps.eligibility = 'failed';
        this.fail("Invalid gold weight");
        return;
      }

      if (this.goldPurity < 70 || this.goldPurity > 100) {
        this.steps.eligibility = 'failed';
        this.fail("Purity must be between 70% - 100%");
        return;
      }

      const ratePerGram = 5000; // approx
      const loanAmount = this.goldWeight * (this.goldPurity / 100) * ratePerGram;

      this.loanService.productId = this.productId;
      this.loanService.sharedLoanAmount = loanAmount;

      this.success(`Eligible for Gold Loan (₹${Math.round(loanAmount)})`);
    }


      //Gold Loan

  

    else {
      this.steps.eligibility = 'failed';
      this.fail("Invalid Product");
    }
  }


  success(message: string) {
    this.steps.eligibility = 'done';
    this.result = message;
    this.eligible = true;
    this.isChecking = false;
    this.allDone = true;
  }


  fail(message: string) {
    this.result = "Not Eligible (" + message + ")";
    this.eligible = false;
    this.isChecking = false;
    this.allDone = false;
  }


  resetSteps() {
    this.steps = {
      cibil: 'pending',
      income: 'pending',
      eligibility: 'pending'
    };

    this.result = "";
    this.eligible = null;
    this.allDone = false;
  }
}
