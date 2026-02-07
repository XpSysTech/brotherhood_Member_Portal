import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageVotesComponent } from './manage-votes.component';

describe('ManageVotesComponent', () => {
  let component: ManageVotesComponent;
  let fixture: ComponentFixture<ManageVotesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageVotesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageVotesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
