import { Component, inject } from '@angular/core';
import { UserAction, UserAudit } from '../../../model/model';
import { AdminService } from '../../../services/admin.service';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-audit-details',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './audit-details.component.html',
  styleUrl: './audit-details.component.css'
})
export class AuditDetailsComponent {
  actions: UserAction[] = [];
  adminServices = inject(AdminService);
  router = inject(Router)

  ngOnInit(): void {
    this.auditDetails();
  }

  auditDetails(): void {
    this.adminServices.getAuditdetails().subscribe((res: any) => {
      this.actions = res.data;
    });
  }
  }
