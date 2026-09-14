import { Component } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  constructor(
    private authService: AuthService,
    private router: Router
  ) { }
  user = {
    email: '',
    password: ''
  };

  login() {
    this.authService.login(this.user.email, this.user.password)
      .subscribe({
        next: (res: any) => {
          if (!res.success) {
            alert(res.message); 
            return;
          }
          localStorage.setItem('user', JSON.stringify(res.user));
       
          localStorage.setItem('role', res.user.roleId == 1 ? 'admin' : 'user');
          this.authService.setloginState(res.token);
          alert("Login successful");
          if (res.user.roleId == 1) {
            this.router.navigate(['/admin-dashboard']).then(() => {
              window.location.reload();
            });
          } else {
            this.router.navigate(['/dashboard']).then(() => {
              window.location.reload();
            }
            );
          }
        },
        error: () => {
          alert("Server error");
        }
      });
  }
}
