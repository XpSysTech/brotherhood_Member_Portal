import { Component, inject } from '@angular/core';
import { FormsModule, NgForm} from '@angular/forms';
import { AccountService } from '../../../../core/services/account-service';
import { Router, RouterLink } from '@angular/router';
import { buildWhatsAppRegistrationLink, buildSmsRegistrationLink } from '../utils/helpers/registration.utils';
import { ToastService } from '../../../../core/services/toast-service';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  // Services
  private account = inject(AccountService);
  private toast = inject(ToastService);
  private router = inject(Router);

  // Form Variables
  protected creds = { email: '', password: '' };
  protected loading = false;

  // Authentication Variables
  protected authError: string | null = null;

  // Member Registration Variables
  protected adminPhone = '+264815732680';
  protected whatsappLink = buildWhatsAppRegistrationLink(this.adminPhone);
  protected smsLink = buildSmsRegistrationLink(this.adminPhone);

  login(form: NgForm) {
    if (form.invalid) return;

    this.loading = true;
    this.authError = null;

    this.account.login(this.creds).subscribe({
      next: (user) => {
        // Show personalized success toast with member's display name
        this.toast.success(`Welcome back, ${user.displayName}!`);
        this.router.navigateByUrl('/member/dashboard');
      },
      error: (err) => {
        if (err.status === 401) {
          this.authError = 'Authorization failed. Invalid email or password.';
          // Show error toast for invalid credentials
          this.toast.error('Invalid email or password. Please try again.');
        } else {
          this.authError = 'Something went wrong. Please try again.';
          // Show error toast for other errors
          this.toast.error('Something went wrong. Please try again.');
        }
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }
}
