import { Component } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  loading = false;

  model = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: '',
  };

  constructor(private router: Router) {}

  register(form: NgForm) {
    if (form.invalid) return;

    this.loading = true;

    // TODO: replace with AccountService.register(...)
    setTimeout(() => {
      this.loading = false;

      // For now, redirect to login
      this.router.navigateByUrl('/login');
    }, 800);
  }
}
