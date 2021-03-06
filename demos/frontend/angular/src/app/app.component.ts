import { Component } from '@angular/core';
import { HealthService } from "./services/health.service";
import {
  catchError,
  Observable,
  of
} from "rxjs";
import { ApiHealthResult } from "./model/api.model";


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.sass']
})
export class AppComponent {
  title = 'query-engine-dashboard';

  apiResult?: ApiHealthResult | null;
  error?: any;
  loading = false;

  constructor(
    private api: HealthService) {
  }

  ngOnInit() {
    this.loadHealth();
  }

  private loadHealth(): Observable<ApiHealthResult> {
    this.loading = true;
    this.error = null;
    this.apiResult = null;
    const result = this.api.getHealth();
    result
      .pipe(catchError(err => {
        this.error = err;
        return of(null)
      }))
      .subscribe(result => {
        this.apiResult = result;
        this.loading = false;
      });
    return result;
  }
}
