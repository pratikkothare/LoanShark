import { Component, OnInit } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-loanproducts',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './loanproducts.component.html',
  styleUrls: ['./loanproducts.component.css']
})
export class LoanproductsComponent implements OnInit {
  loanProducts: any[] = [];
  selectedLoan: any = null;

  private descriptions: { [key: string]: string } = {
    'home loan': 'Fulfill your dream of home ownership with our low-interest home loans, offering flexible repayment tenures and minimal documentation.',
    'car loan': 'Get on the road faster with our competitive car loan rates, quick processing, and financing for both new and pre-owned vehicles.',
    'personal loan': 'Quick financial support for your personal needs, whether it is for travel, medical emergencies, or weddings, with no collateral required.',
    'education loan': 'Invest in your future with loans designed to cover tuition fees and living expenses for top-tier educational institutions worldwide.',
    'gold loan': 'Unlock the value of your gold jewelry for instant cash with high per-gram rates and secure storage.'
  };

  constructor(private auth: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.getProducts();
  }

  getProducts() {
    this.auth.getLoanProducts().subscribe({
      next: (res: any) => {
        this.loanProducts = res.data;
      },
      error: (err) => {
        console.error("Error fetching loan Products", err);
      }
    });
  }

  checkEligibility(loan: any) {
    this.router.navigate(['/check-eligibilty', loan.productId]);
  }

  openModal(loan: any) {
    const productName = loan.productName.trim().toLowerCase();
    const descriptionText = this.descriptions[productName] || 'Tailored financial solutions designed to meet your specific requirements.';

    this.selectedLoan = {
      ...loan,
      displayDescription: descriptionText 
    };
  }

  closeModal() {
    this.selectedLoan = null;
  }
}
