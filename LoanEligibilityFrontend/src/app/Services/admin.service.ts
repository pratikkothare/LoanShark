import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  private baseUrl = 'https://localhost:7156/api';

  constructor(private http: HttpClient) { }

  getUsers(): Observable<any> {
    return this.http.get(`${this.baseUrl}/User`);
  }
  getLoans() {
    return this.http.get(`${this.baseUrl}/Loan/admin`);
  }

  updateLoanStatus(applicationId: number, status: string) {
    return this.http.put(`${this.baseUrl}/Loan/status/${applicationId}?status=${status}`, {});
  }

  getProducts() {
    return this.http.get(`${this.baseUrl}/Loan/products`);
  }

  addProduct(product: any) {
    return this.http.post(`${this.baseUrl}/Loan/product`, product);
  }

  updateProduct(product: any) {
    return this.http.put(`${this.baseUrl}/Loan/product/${product.productId}`, product);
  }

  deleteProduct(id: number) {
    return this.http.delete(`${this.baseUrl}/Loan/product/${id}`);
  }

  getSummary() {
    return this.http.get(`${this.baseUrl}/Loan/analytics/summary`);
  }

  getProductPerformance() {
    return this.http.get(`${this.baseUrl}/Loan/product-performance`);
  }

  getApplicationTrends(fromDate?: string, toDate?: string) {
    let url = `${this.baseUrl}/Loan/analytics/application-trends`;
    if (fromDate && toDate) {
      url += `?fromDate=${fromDate}&toDate=${toDate}`;
    }
    return this.http.get(url);
  }

  getStatusDistribution() {
    return this.http.get(`${this.baseUrl}/Loan/analytics/status-distribution`);
  }

  getLoanDistribution() {
    return this.http.get(`${this.baseUrl}/Loan/analytics/loan-amount-distribution`);
  }

  getInterestAnalysis() {
    return this.http.get(`${this.baseUrl}/Loan/analytics/interest-analysis`);
  }

  getUserInsights() {
    return this.http.get(`${this.baseUrl}/Loan/analytics/user-insights`);
  }

  getRecentActivity() {
    return this.http.get(`${this.baseUrl}/Loan/recent-activity`);
  }

  getTopUser() {
    return this.http.get(`${this.baseUrl}/Loan/top-users`)
  }

  sendNotification(userId: number, message: string) {
    return this.http.post(`${this.baseUrl}/Notification/addnotification/${userId}/${encodeURIComponent(message)}`, {});
  }
}
