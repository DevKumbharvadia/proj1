import { HttpClient, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { environment } from '../../environment/environment.development';
import { Constant } from '../constant/constant';
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  function getCookie(name: string): string | null {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop()?.split(';').shift() || null;
    return null;
  }

  function setCookie(name: string, value: string, minutes: number): void {
    const date = new Date();
    date.setTime(date.getTime() + minutes * 60 * 1000);
    const expires = `expires=${date.toUTCString()}`;
    document.cookie = `${name}=${value}; ${expires}; path=/`;
  }

  const token = getCookie('jwtToken');
  const refreshToken = getCookie('refreshToken');

  // Skip interceptor for the refresh token request
  if (req.url.includes(environment.API_URl + Constant.API_METHOD.AUTH.REFRESH_TOKEN)) {
    return next(req);
  }

  if (req.url.includes(environment.API_URl + Constant.API_METHOD.AUTH.LOGIN)) {
    return next(req);
  }

  if (req.url.includes(environment.API_URl + Constant.API_METHOD.AUTH.REGISTER)) {
    return next(req);
  }

  if (token == null) {
    const http = inject(HttpClient);

    if (refreshToken != null) {
      http.post<any>(environment.API_URl + Constant.API_METHOD.AUTH.REFRESH_TOKEN, { refreshToken })
        .subscribe(res => {
          console.log(res.data);
          setCookie('jwtToken', res.data.jwtToken, 60); // 60 minutes
          setCookie('refreshToken', res.data.refreshToken, (7 * 24 * 60)); // 7 days
        });
    }
  }

  const clonedReq = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`,
    },
  });

  return next(clonedReq);
};
