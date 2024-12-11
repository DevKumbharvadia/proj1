import { NgIf } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MiscService } from '../../../services/misc.service';
import { AuthService } from '../../../services/auth.service';
import { Role } from '../../../model/model';

@Component({
  selector: 'app-add-buyer-info',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule],
  templateUrl: './add-buyer-info.component.html',
  styleUrl: './add-buyer-info.component.css'
})
export class AddBuyerInfoComponent implements OnInit {

  router = inject(Router);
  miscServices = inject(MiscService);
  authService = inject(AuthService);
  roles: Role[] = [];

  contactForm = new FormGroup({
    contactNumber: new FormControl('', [
      Validators.required,
      Validators.pattern(/^[0-9]{10}$/)
    ]),
    address: new FormControl('', [
      Validators.maxLength(250)
    ])
  });

  ngOnInit(): void {
  }

  get contactNumber() {
    return this.contactForm.get('contactNumber');
  }

  get address() {
    return this.contactForm.get('address');
  }

  // buyerInfoExist(){
  //   this.miscServices.buyerInfoExist().subscribe((res:any)=>{
  //     console.log(res);
  //     if (res.data) {
  //       this.router.navigateByUrl('layout');
  //     }
  //   })
  // }

  onContactSubmit(): void {
    sessionStorage.removeItem('cart');
    if (this.contactForm.valid) {
      const formData = new FormData();
      const contactData = this.contactForm.value;
      formData.append('UserId', sessionStorage.getItem('userId') || '');
      formData.append('ContactNumber', contactData.contactNumber || '');
      formData.append('Address', contactData.address || '');

      this.miscServices.setBuyerInfo(formData).subscribe(
        (res: any) => {
          console.log('Form submission successful:', res);
          alert(res.message);
          sessionStorage.removeItem('cart');
          this.router.navigateByUrl("layout/cart");
        }
      );
    } else {
      console.error('Form is invalid');
    }
  }

  loadRoles() {
    this.authService.getAllRoles().subscribe((res: any) => {
      this.roles = res.data;
    });
  }

}
