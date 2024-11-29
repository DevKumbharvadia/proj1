import { Component, inject, OnInit } from '@angular/core';
import { AdminService } from '../../../services/admin.service';
import { Router } from '@angular/router';
import { MiscService } from '../../../services/misc.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-manage-user-roles',
  standalone: true,
  imports: [],
  templateUrl: './manage-user-roles.component.html',
  styleUrl: './manage-user-roles.component.css',
})
export class ManageUserRolesComponent implements OnInit {
  user: any;
  adminServices = inject(AdminService);
  miscServices = inject(MiscService);
  authServices = inject(AuthService);
  router = inject(Router);
  roles: any[] = [];
  userRoles: any[] = [];
  selectedRoles: string[] = [];

  ngOnInit(): void {
    if (this.adminServices.editUser == '') {
      this.router.navigateByUrl('layout/manage-user');
    }
    this.getUserInfo();
    this.getAllRoles();
    this.getUserRoles();
    // this.selectedRolesSet();
  }

  // selectedRolesSet(){
  //   this.roles.forEach(element => {
  //     if(this.userRoles.includes(element.roleName )){
  //       this.selectedRoles.push(element.roleId)
  //       console.log("su"+this.selectedRoles);
  //     }
  //   });
  // }

  getUserInfo() {
    this.adminServices
      .getAllUserInfoById(this.adminServices.editUser)
      .subscribe((res: any) => {
        this.user = res.data;
      });
      
  }

  onRoleChange(event: Event, roleId: string) {
    const isChecked = (event.target as HTMLInputElement).checked;
    if (isChecked) {
      this.selectedRoles.push(roleId);
    } else {
      this.selectedRoles = this.selectedRoles.filter((id) => id !== roleId);
    }
    console.log(this.selectedRoles);

  }

  getAllRoles() {
    this.authServices.getAllRoles().subscribe((res:any)=>{
      this.roles = res.data;
    });
  }

  getUserRoles() {
    this.miscServices.getUserRoles().subscribe((res: any)=>{
      this.userRoles = res.data;
    })
  }
  saveChanges() {
    this.adminServices.rewriteRoles(this.user.userId,this.selectedRoles).subscribe((res:any)=>{
      alert(res.message);
    })
  }
  cancelChanges() {
    this.router.navigateByUrl("layout/manage-user")
  }
}
