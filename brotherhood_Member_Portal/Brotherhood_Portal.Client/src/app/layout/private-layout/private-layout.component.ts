import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MemberHeaderComponent } from "../../portal/shared/member-header/member-header.component";
import { MemberSidebarComponent } from "../../portal/shared/member-sidebar/member-sidebar.component";
import { MemberFooterComponent } from "../../portal/shared/member-footer/member-footer.component";

@Component({
  selector: 'app-private-layout',
  imports: [RouterOutlet, MemberHeaderComponent, MemberSidebarComponent, MemberFooterComponent],
  templateUrl: './private-layout.component.html',
  styleUrl: './private-layout.component.css'
})

export class PrivateLayoutComponent {
  sidebarOpen = false;

  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  closeSidebar() {
    this.sidebarOpen = false;
  }
}
