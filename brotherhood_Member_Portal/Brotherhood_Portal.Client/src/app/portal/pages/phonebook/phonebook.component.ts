import { Component, OnInit } from '@angular/core';
import { MemberPhonebook } from './model/member-phonebook.model';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-phonebook',
  templateUrl: './phonebook.component.html',
  imports: [CommonModule]
})
export class PhonebookComponent implements OnInit {

  members: MemberPhonebook[] = [];
  expandedMemberId: string | null = null;

  loading = true;
  errorMessage: string | null = null;

  readonly baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadMembers();
  }

  private loadMembers(): void {
    this.http
      .get<MemberPhonebook[]>(`${this.baseUrl}members/phonebook`)
      .subscribe({
        next: members => {
          this.members = members;
          this.loading = false;
        },
        error: err => {
          this.loading = false;

          if (err.status === 401) {
            this.errorMessage = 'You are not authorized to view the phonebook.';
          } else {
            this.errorMessage = 'Failed to load phonebook. Please try again later.';
          }
        }
      });
  }

  // void = “this function must not return a value”
  toggle(memberId: string): void {
    this.expandedMemberId =
      this.expandedMemberId === memberId ? null : memberId;
  }

  openWhatsApp(number: string): void {
    const sanitized = number.replace(/\D/g, '');
    window.open(`https://wa.me/${sanitized}`, '_blank');
  }

  frameworkExpanded = false;

  toggleFramework(): void {
    this.frameworkExpanded = !this.frameworkExpanded;
  }
}
