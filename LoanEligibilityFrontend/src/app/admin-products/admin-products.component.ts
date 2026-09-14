import { Component, OnInit } from '@angular/core';

import { AdminService } from '../Services/admin.service';

@Component({

  selector: 'app-admin-products',

  templateUrl: './admin-products.component.html',

  styleUrls: ['./admin-products.component.css']

})

export class AdminProductsComponent implements OnInit {

  products: any[] = [];

  filteredProducts: any[] = [];

  searchText: string = '';


  currentPage: number = 1;

  pageSize: number = 5;


  showModal: boolean = false;

  isEditMode: boolean = false;

  selectedProduct: any = {

    productId: 0,

    productName: '',

    interestRate: null,

    maxAmount: null,

    tenureMonths: null

  };

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {

    this.fetchProducts();

  }

  fetchProducts() {

    this.adminService.getProducts().subscribe({

      next: (res: any) => {

        this.products = res.data || [];

        this.filteredProducts = [...this.products];

      },

      error: () => alert('Failed to load products')

    });

  }


  applyFilter() {

    const term = this.searchText.toLowerCase();

    this.filteredProducts = this.products.filter(p =>

      p.productName.toLowerCase().includes(term)

    );

    this.currentPage = 1;

  }


  get paginatedProducts() {

    const start = (this.currentPage - 1) * this.pageSize;

    return this.filteredProducts.slice(start, start + this.pageSize);

  }

  totalPages(): number {

    return Math.ceil(this.filteredProducts.length / this.pageSize);

  }

  changePage(page: number) {

    this.currentPage = page;

  }

  
  openAddModal() {

    this.isEditMode = false;

    this.selectedProduct = {

      productId: 0,

      productName: '',

      interestRate: null,

      maxAmount: null,

      tenureMonths: null

    };

    this.showModal = true;

  }

  openEditModal(product: any) {

    this.isEditMode = true;

    this.selectedProduct = { ...product };

    this.showModal = true;

  }

  closeModal() {

    this.showModal = false;

  }

  
  saveProduct() {

    if (!this.selectedProduct.productName) {

      alert('Product name required');

      return;

    }

    if (this.isEditMode) {

      this.adminService.updateProduct(this.selectedProduct).subscribe({

        next: () => {

          this.closeModal();

          this.fetchProducts();

        },

        error: () => alert('Update failed')

      });

    } else {

      this.adminService.addProduct(this.selectedProduct).subscribe({

        next: () => {

          this.closeModal();

          this.fetchProducts();

        },
        error: () => alert('Add failed')
      });
    }
  }

  deleteProduct(id: number) {
    if (!confirm('Delete this product?')) return;
    this.adminService.deleteProduct(id).subscribe({
      next: () => this.fetchProducts(),
      error: () => alert('Delete failed')
    });
  }
}
