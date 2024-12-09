import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environment/environment.development';
import { Constant } from '../constant/constant';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  http = inject(HttpClient);
  editUser: string = '';
  auditId: string = '';

  constructor() { }

  //ok
  rewriteRoles(userId: string, roleIds: string[]): Observable<any>{
    return this.http.post<any>(environment.API_URl + Constant.API_METHOD.ADMIN.REWRITE_ROLES + '?userId=' + `${userId}`,roleIds)
  }

  //ok
  assignRole(userId: string, roleId: string): Observable<any>{
    return this.http.post<any>(environment.API_URl + Constant.API_METHOD.USER_ROLE.ASSIGN_ROLE+"?userId="+`${userId}`+"&roleId="+`${roleId}`,{});
  }

  //ok
  getAllAudits(): Observable<any>{
    return this.http.get<any>(environment.API_URl + Constant.API_METHOD.USER_AUDIT.GET_ALL);
  }

  //ok
  getAllUserInfo(): Observable<any>{
    return this.http.get<any>(environment.API_URl + Constant.API_METHOD.USER_INFO.GET_ALL)
  }

  //ok
  GetAllWhiteListedUserInfo(): Observable<any>{
    return this.http.get<any>(environment.API_URl + Constant.API_METHOD.ADMIN.GET_WHITELISTED_USERS)
  }

  //ok
  getAllUserInfoById(Id: string): Observable<any>{
    return this.http.get<any>(environment.API_URl + Constant.API_METHOD.USER_INFO.GET_BY_USER_ID + "?id=" + `${Id}`)
  }

  //ok
  getUserPurchases(Id: string): Observable<any>{
    return this.http.get<any>(environment.API_URl + Constant.API_METHOD.TRANSACTION.GET_TRANSACTION_HISTORY_BY_USER_ID + "?id=" + `${Id}`)
  }

  //ok
  banUser(Id: string): Observable<any>{
    return this.http.post<any>(environment.API_URl + Constant.API_METHOD.ADMIN.BLACKLIST_USER + "?Id=" + `${Id}`,{});
  }

  getAuditdetails(){
    return this.http.get<any>(environment.API_URl + Constant.API_METHOD.USER_ACTION.GET_BY_AUDIT_ID + '?Id=' + `${this.auditId}`);
  }
}
