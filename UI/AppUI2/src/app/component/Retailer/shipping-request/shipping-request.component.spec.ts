import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShippingRequestComponent } from './shipping-request.component';

describe('ShippingRequestComponent', () => {
  let component: ShippingRequestComponent;
  let fixture: ComponentFixture<ShippingRequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShippingRequestComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShippingRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
