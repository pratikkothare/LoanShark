import { Component, OnInit } from '@angular/core';

import { AdminService } from '../Services/admin.service';

@Component({

  selector: 'app-admin-users',

  templateUrl: './admin-users.component.html',

  styleUrls: ['./admin-users.component.css']

})

export class AdminUsersComponent implements OnInit {

  users: any[] = [];

  loading: boolean = false;

  errorMessage: string = '';

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {

    this.fetchUsers();

  }

  fetchUsers(): void {
    this.loading = true;
    this.adminService.getUsers().subscribe({
      next: (res: any) => {
        const data = Array.isArray(res) ? res : res.data || [];
        this.users = data.filter((u:any)=>u.roleId !=1);
        this.loading = false;
      },

      error: (err) => {

        console.error(err);

        this.errorMessage = 'Failed to load users';

        this.loading = false;

      }

    });

  }

}
