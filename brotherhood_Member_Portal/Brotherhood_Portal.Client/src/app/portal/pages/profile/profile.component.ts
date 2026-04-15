import { Component, inject } from '@angular/core';
import { AccountService } from '../../../../core/services/account-service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { createEmptyProfile, UpdateProfileDto } from '../../../../core/interfaces/AccountDto';

@Component({
  selector: 'app-profile',
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent {

  private account = inject(AccountService);

  protected currentUser = this.account.currentUser;
  protected isEditing = false;

  profile: UpdateProfileDto = createEmptyProfile();
  saving = false;

  ngOnInit() {
    this.account.getProfile().subscribe(profile => {
      this.profile = profile;
    });
  }

  saveProfile() {
    this.saving = true;

    this.account.updateProfile(this.profile).subscribe({
      next: () => {
        this.saving = false;
        this.isEditing = false;
      },
      error: () => this.saving = false
    });
  }

  toggleEdit() {
    this.isEditing = !this.isEditing;
  }

}
