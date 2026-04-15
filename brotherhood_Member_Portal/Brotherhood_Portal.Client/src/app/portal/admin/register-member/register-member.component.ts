import { Component, inject } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { AccountService } from '../../../../core/services/account-service';
import { ToastService } from '../../../../core/services/toast-service';
import { RegisterMemberModel } from './model/register-member';

@Component({
  selector: 'app-register-member',
  imports: [FormsModule],
  templateUrl: './register-member.component.html',
  styleUrl: './register-member.component.css'
})
export class RegisterMemberComponent {
  private accountService = inject(AccountService);
  private toast = inject(ToastService);

  // Initialize model with empty values matching the RegisterMemberModel interface
  model: RegisterMemberModel = {
    displayName: '',
    email: '',
    password: '',
    confirmPassword: '',
    dateOfBirth: '',
    contactNumber: '',
    homeAddress: '',
    homeCity: ''
  };

  loading = false;

  createMember(form: NgForm) {
    if (form.invalid) return;

    if (this.model.password !== this.model.confirmPassword) {
      this.toast.error('Passwords do not match');
      return;
    }

    this.loading = true;

    this.accountService.register(this.model).subscribe({
      next: () => {
        this.toast.success('Member account created successfully');
        form.resetForm();
        this.loading = false;
      },
      error: (err: { error: { title: any; }; }) => {
        this.toast.error(
          err?.error?.title || 'Failed to create member account'
        );
        this.loading = false;
      }
    });
  }
}
