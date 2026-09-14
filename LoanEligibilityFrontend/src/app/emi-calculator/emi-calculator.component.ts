import { Component, OnInit } from '@angular/core';
import { LoanDataService } from '../Services/loan-data.service';

@Component({
  selector: 'app-emi-calculator',
  templateUrl: './emi-calculator.component.html',
  styleUrls: ['./emi-calculator.component.css']
})
export class EmiCalculatorComponent implements OnInit {

  loanAmount: number = 0;
  tenureMonths: number = 0;
  income: number = 0;
  loanType: string = "";
  emi: number = 0;
  result: string = "";
  interestRate: number = 0;
  isEligible: boolean= false;
  minEligibleTenure: number = 6;

  totalAmount: number = 0;
  totalInterest: number = 0;
  tenureYears: number = 0;

  constructor(private loanService: LoanDataService) { }

  ngOnInit() {
    this.income = this.loanService.sharedIncome;
    this.loanAmount = this.loanService.sharedLoanAmount;

    const loan = this.getLoanDetails(this.loanService.productId);
    this.loanType = loan.type;
    this.interestRate = loan.rate;

    this.minEligibleTenure = this.getMinTenure();

    if (this.minEligibleTenure === -1) {
      this.tenureMonths = 300;
    } else {
      this.tenureMonths = this.minEligibleTenure;
    }
  }

  getLoanDetails(id: number) {
    switch (id) {
      case 1: return { type: "Home Loan", rate: 8.5 };
      case 2: return { type: "Car Loan", rate: 9.5 };
      case 3: return { type: "Personal Loan", rate: 12 };
      case 4: return { type: "Education Loan", rate: 10 };
      case 5: return { type: "Gold Loan", rate: 13 };
      default: return { type: "Loan", rate: 10 };
    }
  }

  calculateEmi() {

    if (this.tenureMonths <= 0) {
      this.result = "Enter valid tenure";
      return;
    }

    const P = this.loanAmount;
    const r = this.interestRate / (12 * 100);
    const n = this.tenureMonths;

    this.emi = (P * r * Math.pow(1 + r, n)) /
      (Math.pow(1 + r, n) - 1);

    this.emi = Number(this.emi.toFixed(2));

    this.totalAmount = Number((this.emi * n).toFixed(2));
    this.totalInterest = Number((this.totalAmount - P).toFixed(2));
    this.tenureYears = Number((n / 12).toFixed(1));

    const maxEmi = this.income * 0.5;

    if (this.minEligibleTenure !== -1 && this.tenureMonths < this.minEligibleTenure) {
      this.result = "Not Eligible: Increase tenure";
      this.isEligible = false;
    }

    else if (this.minEligibleTenure === -1 && this.tenureMonths === 300) {
      this.result = " Not Eligible max tenure Can be 25 years";
      this.isEligible = false;
    }

    else if (this.emi > maxEmi) {
      this.result = " EMI exceeds 50% of income";
      this.isEligible = false;
    }

    else {
      this.result = "You Can Apply for this Loan";
      this.isEligible = true;
      this.loanService.loanAmount = this.loanAmount;
      this.loanService.tenureMonths = this.tenureMonths;
      
    }
  }

  getMinTenure(): number {
    const P = this.loanAmount;
    const r = this.interestRate / (12 * 100);
    const maxEMI = this.income * 0.5;

    for (let n = 6; n <= 300; n++) {
      const emi = (P * r * Math.pow(1 + r, n)) /
        (Math.pow(1 + r, n) - 1);

      if (emi <= maxEMI) {
        return n;
      }
    }

    return -1;
  }
}
