import { Component } from '@angular/core';
import { NotificationService } from '../Services/notification.service';

@Component({
  selector: 'app-shownotification',
  templateUrl: './shownotification.component.html',
  styleUrls: ['./shownotification.component.css']
})
export class ShownotificationComponent {
  notifications: any[] = [];
  userId: number = 0;

  constructor(private notificationService: NotificationService) { }

  ngOnInit() {
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    this.userId = user?.userId;

    this.loadNotifications();
  }

  loadNotifications() {
    this.notificationService.getNotifications(this.userId).subscribe(res => {
      this.notifications = res.data;
    });
  }

  markAsRead(id: number) {
    this.notificationService.markAsRead(id).subscribe(() => {
      this.loadNotifications(); // refresh
    });
  }

  getCleanMessage(msg: string) {
    return decodeURIComponent(msg);
  }
}

