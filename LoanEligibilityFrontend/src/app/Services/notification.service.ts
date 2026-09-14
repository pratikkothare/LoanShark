import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {


  constructor(private http: HttpClient) { }

  getNotifications(userId: number) {
    return this.http.get<any>(`https://localhost:7156/api/Notification/getNotifications/${userId}`);
  }

  markAsRead(id: number) {
    return this.http.put(`https://localhost:7156/api/Notification/markAsRead/${id}`, {});
  }

  getUnreadCount(userId: number) {
    return this.http.get<any>(`https://localhost:7156/api/Notification/unreadCount/${userId}`);
  }
}

