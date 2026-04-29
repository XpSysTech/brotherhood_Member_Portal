import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { ManageMemberFinanceComponent } from './manage-member-finance.component';
import { FinanceService } from '../../../../../core/services/finance-service';
import { MemberByName } from '../../../../../core/interfaces/Member';
import { FinanceRecord } from '../../../../../core/interfaces/FinanceDto';
import { SpyObj } from 'jasmine';

describe('ManageMemberFinanceComponent', () => {
  let component: ManageMemberFinanceComponent;
  let fixture: ComponentFixture<ManageMemberFinanceComponent>;
  let financeServiceSpy: SpyObj<FinanceService>;

  const mockMembers: MemberByName[] = [
    { id: '2', displayName: 'Zulu' },
    { id: '1', displayName: 'Alpha' }
  ];

  const mockHistory: FinanceRecord[] = [
    {
      financeId: 1, // ✅ REQUIRED
      savingsAmount: 1000,
      opsContributionAmount: 100,
      depositDate: new Date(),
      status: 'Approved'
    }
  ];

  beforeEach(async () => {

    financeServiceSpy = jasmine.createSpyObj('FinanceService', [
      'getMembersForDropdown',
      'addDeposit',
      'getMemberHistory'
    ]);

    await TestBed.configureTestingModule({
      imports: [ManageMemberFinanceComponent],
      providers: [
        { provide: FinanceService, useValue: financeServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ManageMemberFinanceComponent);
    component = fixture.componentInstance;
  });

  // --------------------------------------------
  // BASIC CREATION
  // --------------------------------------------

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  // --------------------------------------------
  // LOAD MEMBERS
  // --------------------------------------------

  it('should load and sort members alphabetically on init', () => {
    financeServiceSpy.getMembersForDropdown.and.returnValue(of(mockMembers));

    component.ngOnInit();

    expect(financeServiceSpy.getMembersForDropdown).toHaveBeenCalled();
    expect(component.members[0].displayName).toBe('Alpha');
    expect(component.members[1].displayName).toBe('Zulu');
  });

  it('should handle member loading error', () => {
    financeServiceSpy.getMembersForDropdown.and.returnValue(
      throwError(() => new Error('API Error'))
    );

    component.ngOnInit();

    expect(component.feedback).toBe('Failed to load members.');
  });

  // --------------------------------------------
  // SUBMIT DEPOSIT
  // --------------------------------------------

  it('should not submit deposit if no member selected', () => {
    component.selectedMemberId = null;

    component.submitDeposit();

    expect(financeServiceSpy.addDeposit).not.toHaveBeenCalled();
  });

  it('should submit deposit when valid', () => {
    component.selectedMemberId = '1';

    financeServiceSpy.addDeposit.and.returnValue(
      of({ message: 'Deposit submitted successfully.' })
    );

    component.submitDeposit();

    expect(financeServiceSpy.addDeposit).toHaveBeenCalled();
  });

  it('should handle deposit submission error', () => {
    component.selectedMemberId = '1';

    financeServiceSpy.addDeposit.and.returnValue(
      throwError(() => ({ error: { message: 'Failed' } }))
    );

    component.submitDeposit();

    expect(component.feedback).toBe('Failed');
  });

  // --------------------------------------------
  // LOAD HISTORY
  // --------------------------------------------

  it('should load member history', () => {
    component.selectedMemberId = '1';

    financeServiceSpy.getMemberHistory.and.returnValue(of(mockHistory));

    component.loadHistory();

    expect(financeServiceSpy.getMemberHistory).toHaveBeenCalledWith('1');
    expect(component.history.length).toBe(1);
  });

  it('should handle history loading error', () => {
    component.selectedMemberId = '1';

    financeServiceSpy.getMemberHistory.and.returnValue(
      throwError(() => new Error('Error'))
    );

    component.loadHistory();

    expect(component.feedback).toBe('Failed to load history.');
  });
});
