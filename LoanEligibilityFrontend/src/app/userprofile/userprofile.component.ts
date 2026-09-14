import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-userprofile',
  templateUrl: './userprofile.component.html',
  styleUrls: ['./userprofile.component.css']
})
export class UserprofileComponent implements OnInit {
  firstName: string = "";
  lastName: string = "";
  email: string = "";
  roleId: number = 0;
  roleName: string = '';

  
  constructor() { }

  ngOnInit(): void {
    this.loadUser();
  }

  loadUser() {
    const data = localStorage.getItem('user');

    if (data) {
      const user = JSON.parse(data);
      const fullName = user.name || user.fullName || user.userName || '';
      
      this.email = user.email || '';
      this.splitName(fullName);

      this.roleId = user.roleId;
      this.roleName = this.roleId == 1 ? 'Admin' : 'User';
    }
  }

  splitName(fullName: string) {
    const parts = fullName.trim().split(' ');
    this.firstName = parts[0] || '';
    this.lastName = parts.slice(1).join('') || '';

  }

}
