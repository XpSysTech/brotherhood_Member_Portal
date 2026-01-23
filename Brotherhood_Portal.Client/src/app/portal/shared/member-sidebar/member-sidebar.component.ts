import { NgIf } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-member-sidebar',
  imports: [NgIf, RouterLink, RouterLinkActive],
  templateUrl: './member-sidebar.component.html',
  styleUrl: './member-sidebar.component.css'
})
export class MemberSidebarComponent {
  @Input() open = false;              // mobile drawer open/close
  @Output() close = new EventEmitter<void>();

  closeSidebar() {
    this.close.emit();
  }
}
