import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApproveDepositsComponent } from './approve-deposits.component';

describe('ApproveDepositsComponent', () => {
  let component: ApproveDepositsComponent;
  let fixture: ComponentFixture<ApproveDepositsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApproveDepositsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ApproveDepositsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
