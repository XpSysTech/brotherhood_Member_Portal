import { Component, inject } from '@angular/core';
import { AccountService } from '../../../../core/services/account-service';

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  private account = inject(AccountService);
  protected currentUser = this.account.currentUser;
  

}
