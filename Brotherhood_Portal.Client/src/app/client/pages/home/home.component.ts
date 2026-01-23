// home.component.ts (required for modal)
import { Component } from '@angular/core';

type ModalKey = 'forums' | 'support' | 'partners' | 'updates';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [],
  templateUrl: './home.component.html',
})
export class HomeComponent {
  modalOpen = false;
  modalTitle = '';
  modalBody = '';

  private modalContent: Record<ModalKey, { title: string; body: string }> = {
    forums: {
      title: 'Forums & Dialogues',
      body: 'We host and support public discussions that bring stakeholders together around constructive themes. Public notes and summaries may be shared when appropriate.',
    },
    support: {
      title: 'Community Support',
      body: 'We highlight and support initiatives with social value by amplifying credible efforts and encouraging collaboration between civic actors and partners.',
    },
    partners: {
      title: 'Partner Spotlight',
      body: 'We share verified progress and milestones from collaborating organizations. The goal is to increase visibility for social good and encourage responsible engagement.',
    },
    updates: {
      title: 'Public Updates',
      body: 'We publish public-facing notices, short summaries, and partner highlights. These updates reflect visible activity and community momentum.',
    },
  };

  openModal(key: ModalKey) {
    const data = this.modalContent[key];
    this.modalTitle = data.title;
    this.modalBody = data.body;
    this.modalOpen = true;
    document.body.style.overflow = 'hidden';
  }

  closeModal() {
    this.modalOpen = false;
    document.body.style.overflow = '';
  }
}
