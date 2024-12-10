import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ILoginModel } from '../../../model/interface';
import { LoginModel } from '../../../model/model';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { MiscService } from '../../../services/misc.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnInit {
  authServices = inject(AuthService);
  router = inject(Router);
  loginData: ILoginModel = new LoginModel();
  miscServices = inject(MiscService);

  ngOnInit(): void {
  }

  onLogin() {
    this.authServices.onLogin(this.loginData).subscribe(
      (res: any) => {
        if(res.success == false){
          alert(res.message)
        }
        else {
          this.miscServices.setCookie('jwtToken', res.data.jwtToken, 60); // 60 minutes
          this.miscServices.setCookie('refreshToken', res.data.refreshToken, (7 * 24 * 60)); // 7 days
          sessionStorage.setItem('userId', res.data.userId);
          this.router.navigateByUrl('layout');
        }
      },
      (err: any) => {
        alert('Login Credentials Incorrect');
      }
    );
  }
}
