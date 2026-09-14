import { Injectable } from '@angular/core';
import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { throwError, BehaviorSubject } from 'rxjs';
import { ThemeService } from './theme.service';

@Injectable({

  providedIn: 'root'

})

export class AuthService {

  constructor(private http: HttpClient) { }

  //Authentications
  login(email: string, password: string) {

    return this.http.post(`https://localhost:7156/api/User/login?email=${email}&password=${password}`, {}).pipe(catchError(this.handleError));

  }
  register(data: any) {
    return this.http.post(`https://localhost:7156/api/User/register?name=${data.FullName}&email=${data.Email}&password=${data.PasswordHash}`, {}).pipe(catchError(this.handleError));
  }

  getLoanProducts() {
    return this.http.get('https://localhost:7156/api/Loan/products').pipe(catchError(this.handleError));
  }

  resetPassword(data: any) {

    return this.http.post(

      `https://localhost:7156/api/User/reset-password?email=${data.email}&newPassword=${data.newPassword}`, {}).pipe(catchError(this.handleError));

  }

  // Loan
  applyLoan(formdata: FormData) {
    return this.http.post(
      `https://localhost:7156/api/Loan/apply`, formdata).pipe(catchError(this.handleError));
  }


  //NavBar logic
  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());
  isLoggedIn$ = this.loggedIn.asObservable();

  private hasToken(): boolean {
    return !!localStorage.getItem('token');
  }

  setloginState(token: string) {
    localStorage.setItem('token', token);
    this.loggedIn.next(true);
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('user');
    this.loggedIn.next(false);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  isAdmin(): boolean {
    return localStorage.getItem('role') === 'admin';
  }

  withdrawApplication(id: number) {
    return this.http.post('https://localhost:7156/api/Loan/withdraw', id);
  }


  // Error Handler
  private handleError(error: any) {
    console.error(error);
    return throwError(()=>error);

  }
}
