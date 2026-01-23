import { Routes } from '@angular/router';
import { PublicLayoutComponent } from './layout/public-layout/public-layout.component';
import { PrivateLayoutComponent } from './layout/private-layout/private-layout.component';

export const routes: Routes = [
    // PUBLIC ROUTES
    {
        path: '',
        component: PublicLayoutComponent,
        children: [
            // PRIMARY LINKS
            {
                path: '',
                loadComponent: () =>
                import('./client/pages/home/home.component').then((m) => m.HomeComponent),
            },
            {
                path: 'society',
                loadComponent: () =>
                import('./client/pages/society/society.component').then((m) => m.SocietyComponent),
            },
            {
                path: 'contact',
                loadComponent: () =>
                import('./client/pages/contact/contact.component').then((m) => m.ContactComponent),
            },
            {
                path: 'login',
                loadComponent: () =>
                import('./client/shared/login/login.component').then((m) => m.LoginComponent),
            },
            {
                path: 'register',
                loadComponent: () =>
                import('./client/shared/register/register.component').then((m) => m.RegisterComponent),
            },

            // FOOTER LINKS
            {
                path: 'legal',
                loadComponent: () =>
                import('./client/pages/legal/legal.component').then((m) => m.LegalComponent),
            },
            {
                path: 'sitemap',
                loadComponent: () =>
                import('./client/pages/sitemap/sitemap.component').then((m) => m.SitemapComponent),
            },
            {
                path: 'privacy',
                loadComponent: () =>
                import('./client/pages/privacy/privacy.component').then((m) => m.PrivacyComponent),
            },
            {
                path: 'partners',
                loadComponent: () =>
                import('./client/pages/partners/partners.component').then((m) => m.PartnersComponent),
            },
        ],
    },

    // PRIVATE MEMBER ROUTES
    {
    path: 'member',
    component: PrivateLayoutComponent,
    children: [
        {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard',
        },
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

        // MEMBER COMMUNITY ROUTES
        {
        path: 'phonebook',
        loadComponent: () =>
            import('./portal/pages/phonebook/phonebook.component')
            .then(m => m.PhonebookComponent),
        },
        {
        path: 'opportunities',
        loadComponent: () =>
            import('./portal/pages/opportunities/opportunities.component')
            .then(m => m.OpportunitiesComponent),
        },
        {
        path: 'ideas',
        loadComponent: () =>
            import('./portal/pages/ideas/ideas.component')
            .then(m => m.IdeasComponent),
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
    ],
    },


    // FALLBACK ROUTE
    { path: '**', redirectTo: '' },
];


