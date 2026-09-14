import { Component, OnInit } from '@angular/core';
import { ThemeService } from './Services/theme.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'LoanEligibilityFrontend';

  constructor(private theme: ThemeService) {}

  ngOnInit() {
    this.theme.loadTheme();
  }
}
