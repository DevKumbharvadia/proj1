import { HttpClient } from '@angular/common/http';
import { ENVIRONMENT_INITIALIZER, inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment.development';
import { Constant } from '../constant/constant';
import { UserActionRequest } from '../model/model';

@Injectable({
  providedIn: 'root'
})
export class MiscService {

  http = inject(HttpClient);

  constructor() { }

  setCookie(name: string, value: string, minutes: number): void {
    const date = new Date();
    date.setTime(date.getTime() + minutes * 60 * 1000);
    const expires = `expires=${date.toUTCString()}`;
    document.cookie = `${name}=${value}; ${expires}; path=/`;
  }

  buyerInfoExist(): Observable<any>{
    return this.http.get(environment.API_URl + Constant.API_METHOD.BUYER_INFO.BUYER_INFO_EXIST+"?Id="+sessionStorage.getItem('userId'));
  }

  setBuyerInfo(Data: FormData):  Observable<any>{
    return this.http.post(environment.API_URl + Constant.API_METHOD.BUYER_INFO.ADD_BUYER_INFO, Data);
  }

  getUserRoles():  Observable<any>{
    var Id: string = sessionStorage.getItem('userId') ?? '';
    return this.http.get(environment.API_URl + Constant.API_METHOD.ROLE.GET_BY_ID + "?userId=" + `${Id}`);
  }

  loadSalesData():  Observable<any>{
    return this.http.get(environment.API_URl + Constant.API_METHOD.SELLER.SALES_DATA_BY_ID + `?sellerId=${sessionStorage.getItem('userId')}`);
  }

  AddUserAction(Action: string){
    var UA: UserActionRequest = new UserActionRequest(sessionStorage.getItem('userId')??'',Action);
    return this.http.post(environment.API_URl + Constant.API_METHOD.USER_ACTION.ADD_ACTION,UA);
  }
}
