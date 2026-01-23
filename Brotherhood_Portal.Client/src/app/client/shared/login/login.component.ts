import { Component, inject } from '@angular/core';
import { FormsModule, NgForm} from '@angular/forms';
import { AccountService } from '../../../../core/services/account-service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  // Variables
  private account = inject(AccountService);
  protected creds = { email: '', password: '' };
  protected loading = false;
  private router = inject(Router);

  login(form: NgForm) {
    if (form.invalid) return;

    this.loading = true;

    this.account.login(this.creds).subscribe({
      next: () => {
        this.router.navigateByUrl('/member/dashboard');
      },
      complete: () => {
        this.loading = false;
      }
    });
  }
}
