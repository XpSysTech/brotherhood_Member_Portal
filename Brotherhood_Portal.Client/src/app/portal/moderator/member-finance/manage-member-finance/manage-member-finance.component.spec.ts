import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageMemberFinanceComponent } from './manage-member-finance.component';

describe('ManageMemberFinanceComponent', () => {
  let component: ManageMemberFinanceComponent;
  let fixture: ComponentFixture<ManageMemberFinanceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageMemberFinanceComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageMemberFinanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
