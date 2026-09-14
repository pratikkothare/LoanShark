import { Component, OnInit } from '@angular/core';
import { AdminService } from '../Services/admin.service';
declare var Chart: any;
@Component({
  selector: 'app-admin-analytics',
  templateUrl: './admin-analytics.component.html',
  styleUrls: ['./admin-analytics.component.css']
})
export class AdminAnalyticsComponent implements OnInit {

  filter = {
    range: '7',
    startDate: '',
    endDate: ''
  };

  summary: any = {};

  productPerformance: any[] = [];
  userInsights: any[] = [];
  recentActivity: any[] = [];

  trendChartInstance: any;
  statusChartInstance: any;
  loanChartInstance: any;
  interestChartInstance: any;
  productChartInstance: any;

  constructor(private service: AdminService) { }

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll() {
    this.loadSummary();
    this.loadTables();
    this.loadTrendChart();
    this.loadLoanChart();
    this.loadStatusChart();
    this.loadProductChart();
    this.loadInterestChart();
  }

  getSafeArray(data: any): any[] {
    return Array.isArray(data) ? data : [];
  }

  loadSummary() {
    this.service.getSummary().subscribe((res: any) => {
      this.summary = res?.data || {};
    });
  }

  loadTables() {
    this.service.getProductPerformance().subscribe((res: any) => {
      this.productPerformance = this.getSafeArray(res?.data);
    });

    this.service.getUserInsights().subscribe((res: any) => {
      this.userInsights = this.getSafeArray(res?.data);
    });

    this.service.getRecentActivity().subscribe((res: any) => {
      this.recentActivity = this.getSafeArray(res?.data);
    });
  }

  loadTrendChart() {
    this.service.getApplicationTrends().subscribe((res: any) => {
      const data = this.getSafeArray(res?.data);
      if (!data.length) return;

      if (this.trendChartInstance) {
        this.trendChartInstance.destroy();
      }

      this.trendChartInstance = new Chart('trendChart', {
        type: 'line',
        data: {
          labels: data.map((x: any) => x.date),
          datasets: [{
            label: 'Applications',
            data: data.map((x: any) => x.count || 0),
            borderColor: '#673AB7',
            fill: false
          }]
        }
      });
    });
  }

  loadProductChart() {
    this.service.getProductPerformance().subscribe((res: any) => {
      const data = this.getSafeArray(res?.data);
      if (!data.length) return;

      if (this.productChartInstance) {
        this.productChartInstance.destroy();
      }

      this.productChartInstance = new Chart('productChart', {
        type: 'bar',
        data: {
          labels: data.map((x: any) => x.productName),
          datasets: [{
            label: 'Applications',
            data: data.map((x: any) => x.totalApplications || 0),
            backgroundColor: '#42A5F5'
          }]
        }
      });
    });
  }

  loadLoanChart() {
    this.service.getLoanDistribution().subscribe((res: any) => {

      const d = res?.data || {};

      const labels = [
        '0 - 1L',
        '1L - 5L',
        '5L - 10L',
        '10L+'
      ];

      const values = [
        d.range1 || 0,
        d.range2 || 0,
        d.range3 || 0,
        d.range4 || 0
      ];

      if (this.loanChartInstance) {
        this.loanChartInstance.destroy();
      }

      this.loanChartInstance = new Chart('loanChart', {
        type: 'bar',
        data: {
          labels: labels,
          datasets: [{
            label: 'Loans',
            data: values,
            backgroundColor: '#2196F3'
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false
        }
      });
    });
  }

  loadStatusChart() {
    this.service.getStatusDistribution().subscribe((res: any) => {

      const data = res?.data || {};

      const labels = ['Approved', 'Rejected', 'Pending'];
      const values = [
        data.approved || 0,
        data.rejected || 0,
        data.pending || 0
      ];

      if (this.statusChartInstance) {
        this.statusChartInstance.destroy();
      }

      this.statusChartInstance = new Chart('statusChart', {
        type: 'doughnut',
        data: {
          labels: labels,
          datasets: [{
            data: values,
            backgroundColor: ['#4CAF50', '#F44336', '#FFC107']
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          cutout: '60%'
        }
      });
    });
  }

  loadInterestChart() {
    this.service.getInterestAnalysis().subscribe((res: any) => {
      const data = this.getSafeArray(res?.data);
      if (!data.length) return;

      if (this.interestChartInstance) {
        this.interestChartInstance.destroy();
      }

      this.interestChartInstance = new Chart('interestChart', {
        type: 'pie',
        data: {
          labels: data.map((x: any) => x.productName),
          datasets: [{
            data: data.map((x: any) => x.totalApplications || 0),
            backgroundColor: ['#4CAF50', '#FF6384', '#FFCE56', '#36A2EB']
          }]
        }
      });
    });
  }

  applyFilters() {
    this.loadAll();
  }
}
