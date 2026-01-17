import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal, Signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { lastValueFrom } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  private http = inject(HttpClient);
  protected title = 'Brotherhood_Portal.Client';
  
  // We you want to make something available to the compoenent template, declare it as public or protected
  // protected members: any[] = [];
  protected members = signal<any>([]);

  // Make an HTTP GET request to the API endpoint on component initialization
  async ngOnInit() {
    this.members.set(await this.getMembers());
  }

  // Getting the members data using a promise - use this when you want to use async/await
  protected async getMembers(){
    try {
      return await lastValueFrom(this.http.get('https://localhost:5001/api/members'));
    }
    catch (error) {
      console.error('API Error:', error);
      throw error;
    }
  }

}
