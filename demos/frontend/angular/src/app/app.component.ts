import { Component } from '@angular/core';
import {ApiService} from "./services/api.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.sass']
})
export class AppComponent {
  title = 'query-engine-dashboard';

  constructor(private api: ApiService) {
  }

  ngOnInit() {
    this.api.get<any>('health').subscribe(r => {
      console.warn(r);
    });
  }
}
