import { Component, inject, OnInit } from '@angular/core';
import { Role } from '../../../model/model';
import { AuthService } from '../../../services/auth.service';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminService } from '../../../services/admin.service';
import { Router } from '@angular/router';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  authService = inject(AuthService);
  adminService = inject(AdminService);
  roles: Role[] = [];
  router = inject(Router)

  registerForm = new FormGroup(
    {
      username: new FormControl('', [
        Validators.required,
        Validators.minLength(3),
        Validators.pattern(/^\S*$/)
      ]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(6),
      ]),
      confirmPassword: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
      roleId: new FormControl('', Validators.required),
    },
  );

  ngOnInit(): void {
    this.loadRoles();
  }

  get username() {
    return this.registerForm.get('username');
  }

  get password() {
    return this.registerForm.get('password');
  }

  get confirmPassword() {
    return this.registerForm.get('confirmPassword');
  }

  get email() {
    return this.registerForm.get('email');
  }

  get roleId() {
    return this.registerForm.get('roleId');
  }

  onRegister(): void {
    if (this.registerForm.invalid) {
      alert('Form is invalid');
      return;
    }

    if(this.registerForm.get('password')?.value != this.registerForm.get('confirmPassword')?.value){
      alert("Conform Password and Password Don't match")
      return;
    }

    this.authService
      .onRegister(
        this.registerForm.get('username')?.value ?? '',
        this.registerForm.get('password')?.value ?? '',
        this.registerForm.get('email')?.value ?? ''
      )
      .subscribe(
        (res: any) => {
          if (!res.success) {
            alert(res.message);
            return;
          }

          const userId = res.data?.userId;

          this.adminService
            .assignRole(userId, this.registerForm.get('roleId')?.value ?? '')
            .subscribe(
              (roleRes: any) => {
                console.log('Role assigned:', roleRes);
                alert('Registration and role assignment successful!');
                this.router.navigateByUrl('login'); // Navigate only after success
              },
              (error) => {
                console.error('Error during role assignment:', error);
                alert('An error occurred while assigning the role.');
              }
            );
        },
        (error) => {
          console.error('Error during registration:', error);
          alert('An error occurred during registration.');
        }
      );
  }

  loadRoles() {
    this.authService.getAllRoles().subscribe((res: any) => {
      this.roles = res.data;
    });
  }
}
