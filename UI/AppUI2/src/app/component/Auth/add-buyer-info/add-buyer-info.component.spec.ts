import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddBuyerInfoComponent } from './add-buyer-info.component';

describe('AddBuyerInfoComponent', () => {
  let component: AddBuyerInfoComponent;
  let fixture: ComponentFixture<AddBuyerInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddBuyerInfoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddBuyerInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
