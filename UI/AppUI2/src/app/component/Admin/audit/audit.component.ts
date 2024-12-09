import { Component, inject, OnInit } from '@angular/core';
import { AdminService } from '../../../services/admin.service';
import { UserAudit } from '../../../model/model';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-audit',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './audit.component.html',
  styleUrl: './audit.component.css'
})
export class AuditComponent implements OnInit {
  audits: UserAudit[] = [];
  adminServices = inject(AdminService)
  router = inject(Router);

  ngOnInit(): void {
      this.getAllAudits();
  }

  getAllAudits(){
    this.adminServices.getAllAudits().subscribe((res: any)=>{
      this.audits = res.data;
    });
  }

  auditDetails(Id: string){
    this.adminServices.auditId = Id;
    this.router.navigateByUrl("layout/audit-details")
  }

}
