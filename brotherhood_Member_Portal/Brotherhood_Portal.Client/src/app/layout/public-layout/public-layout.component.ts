import { Component } from '@angular/core';
import { HeaderComponent } from "../../client/shared/header/header.component";
import { FooterComponent } from "../../client/shared/footer/footer.component";
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-public-layout',
  imports: [HeaderComponent, RouterOutlet, FooterComponent],
  templateUrl: './public-layout.component.html',
  styleUrls: ['./public-layout.component.css']
})

export class PublicLayoutComponent {

}
