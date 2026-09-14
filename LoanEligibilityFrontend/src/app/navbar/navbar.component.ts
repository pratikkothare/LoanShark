import { Component, HostListener } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { Router } from '@angular/router';
import { NotificationService } from '../Services/notification.service';
@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html'
})
export class NavbarComponent {

  isLoggedIn = false;
  isDropdownOpen = false;
  userName: string = 'User';
  userId: number = 0;
  notifications: any[] = [];
  unreadCount: number = 0;

  constructor(private auth: AuthService, private router: Router, private notificationService: NotificationService)
  {
    this.auth.isLoggedIn$.subscribe(status => {
      this.isLoggedIn = status;
    });
  }

  ngOnInit() {
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    this.userName = user.fullName || 'User';
    this.userId = user.userId;
    if (this.userId) {
      this.loadNotifications();
      this.loadUnreadCount();
    }

  }

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
    if (this.isDropdownOpen) {
      this.loadUnreadCount();
    }
 }

 //If user clicks outside this dropdown closes
  @HostListener('document:click', ['$event'])
      onClickOutside(event: any) {
            if (!event.target.closest('.profile-dropdown')) {
                    this.isDropdownOpen = false;
            }
   }
          
  logout() {
    this.auth.logout();
    localStorage.removeItem('loaniq_chat');
    this.router.navigate(['/']).then(() => {
      window.location.reload();
    });
  }
 

  goHome() {
    const role = localStorage.getItem("role");
    if (role === "admin") {
      this.router.navigate(['/admin-dashboard']);
    }
    else {
      this.router.navigate(['/dashboard']);
    }
  }

  goProfile() {
    this.router.navigate(['/profile']);
    this.isDropdownOpen = false;
  }

  loadNotifications() {
    this.notificationService.getNotifications(this.userId).subscribe({
      next: (res) => {
        this.notifications = res.data;
      },
      error:(err)=>{
        console.error(err)
      }
    })
  }

  goToNotifications() {
    this.router.navigate(['/notification']);
    this.isDropdownOpen = false;
  }

  loadUnreadCount() {
    this.notificationService.getUnreadCount(this.userId).subscribe(
      res => {
        if (res.success) {
          this.unreadCount = res.count;
        }
       
      }
    );
  }
}
