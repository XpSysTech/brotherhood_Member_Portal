import { Routes } from '@angular/router';
import { PublicLayoutComponent } from './layout/public-layout/public-layout.component';
import { PrivateLayoutComponent } from './layout/private-layout/private-layout.component';
import { authGuard } from '../core/guard/auth.guard';
import { fallbackGuard } from '../core/guard/fallback-guard';

export const routes: Routes = [

  // =========================
  // PUBLIC ROUTES
  // =========================
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./client/pages/home/home.component')
            .then(m => m.HomeComponent),
      },
      {
        path: 'society',
        loadComponent: () =>
          import('./client/pages/society/society.component')
            .then(m => m.SocietyComponent),
      },
      {
        path: 'contact',
        loadComponent: () =>
          import('./client/pages/contact/contact.component')
            .then(m => m.ContactComponent),
      },
      {
        path: 'login',
        loadComponent: () =>
          import('./client/shared/login/login.component')
            .then(m => m.LoginComponent),
      },
      // Footer
      {
        path: 'legal',
        loadComponent: () =>
          import('./client/pages/legal/legal.component')
            .then(m => m.LegalComponent),
      },
      {
        path: 'privacy',
        loadComponent: () =>
          import('./client/pages/privacy/privacy.component')
            .then(m => m.PrivacyComponent),
      },
    ],
  },

  // =========================
  // PRIVATE MEMBER ROUTES
  // =========================
  {
    path: 'member',
    component: PrivateLayoutComponent,
    canActivate: [authGuard], // 🔐 everything under /member requires login
    children: [

      // Default redirect
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard',
      },

      // ===== MEMBER ROUTES (All roles)
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./portal/pages/dashboard/dashboard.component')
            .then(m => m.DashboardComponent),
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./portal/pages/profile/profile.component')
            .then(m => m.ProfileComponent),
      },
      {
        path: 'phonebook',
        loadComponent: () =>
          import('./portal/pages/phonebook/phonebook.component')
            .then(m => m.PhonebookComponent),
      },
      {
        path: 'constitution',
        loadComponent: () =>
          import('./portal/pages/constitution/constitution.component')
            .then(m => m.ConstitutionComponent),
      },
      {
        path: 'events',
        loadComponent: () =>
          import('./portal/pages/events/events.component')
            .then(m => m.EventsComponent),
      },
      {
        path: 'voting',
        loadComponent: () =>
          import('./portal/pages/voting/voting.component')
            .then(m => m.VotingComponent),
      },

      // ===== MODERATOR ROUTES
      {
        path: 'moderation',
        canActivate: [authGuard],
        data: { roles: ['Moderator', 'Admin'] },
        children: [
          {
            path: 'finance',
            loadComponent: () =>
              import('./portal/moderator/member-finance/manage-member-finance/manage-member-finance.component')
                .then(m => m.ManageMemberFinanceComponent),
          },
          {
            path: 'approval',
            loadComponent: () =>
              import('./portal/moderator/member-finance/approve-deposits/approve-deposits.component')
                .then(m => m.ApproveDepositsComponent),
          },
          {
            path: 'moderation',
            loadComponent: () =>
              import('./portal/moderator/manage-votes/manage-votes.component')
                .then(m => m.ManageVotesComponent),
          },
        ],
      },

      // ===== ADMIN ROUTES
      {
        path: 'admin',
        canActivate: [authGuard],
        data: { roles: ['Admin'] },
        children: [
          {
            path: 'members',
            loadComponent: () =>
              import('./portal/admin/manage-members/manage-members.component')
                .then(m => m.ManageMembersComponent),
          },
          {
            path: 'register',
            loadComponent: () =>
              import('./portal/admin//register-member/register-member.component')
                .then(m => m.RegisterMemberComponent),
          },
          {
            path: 'settings',
            loadComponent: () =>
              import('./portal/admin/system-settings/system-settings.component')
                .then(m => m.SystemSettingsComponent),
          },
        ],
      },
    ],
  },

  // =========================
  // FALLBACK ROUTE
  // =========================
  {
    path: '**',
    canActivate: [fallbackGuard],
    component: PublicLayoutComponent
  }
];
