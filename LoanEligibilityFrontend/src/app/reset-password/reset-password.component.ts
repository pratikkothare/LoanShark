import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../Services/auth.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent {


    constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  reset = {
    email: '',
    newPassword: '',
    confirmPassword: ''
  };

  resetPassword() {

    if (this.reset.newPassword !== this.reset.confirmPassword) {
      alert("Passwords do not match");
      return;
    }

    this.authService.resetPassword(
      this.reset).subscribe({
        next: (res: any) => { 
          if (res.success)
          {
            alert("Password updated successfully");
            this.router.navigate(['/login']);
          }
          else
          {
            alert(res.message);
          }
        },
      error: () => {
        alert("Wrong");
      }
    });
  }
}
