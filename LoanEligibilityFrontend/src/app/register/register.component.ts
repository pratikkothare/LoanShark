import { Component } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {

  constructor(private auth: AuthService, private router: Router) { }

  user = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: ''
  };

  isValidEmail(): boolean {
    return !!this.user.email && this.user.email.endsWith('@gmail.com');
  }

  register() {
 
    if (this.user.password !== this.user.confirmPassword) {
      alert("Passwords do not match ");
      return;
    }

    const body = {
      FullName: this.user.firstName + ' ' + this.user.lastName,
      Email: this.user.email,
      PasswordHash: this.user.password
    };

    console.log("Sending:", body);

    this.auth.register(body).subscribe({
      next: () => {
        alert("Registered Successfully! Please Login");
        this.router.navigate(['/login'])
      },
      error: (err) => {
        console.log(err);
        alert("Registration Failed");
      }
    });
  }
}
