import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoanDataService {
  sharedIncome: number = 0;
  sharedLoanAmount: number = 0;
  productId: number = 0;
  loanAmount: number = 0;
  tenureMonths: number = 0;
  creditScore: number = 0;
  constructor() { }
}

