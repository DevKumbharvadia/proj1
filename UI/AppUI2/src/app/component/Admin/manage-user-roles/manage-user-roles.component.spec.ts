import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageUserRolesComponent } from './manage-user-roles.component';

describe('ManageUserRolesComponent', () => {
  let component: ManageUserRolesComponent;
  let fixture: ComponentFixture<ManageUserRolesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageUserRolesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageUserRolesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
