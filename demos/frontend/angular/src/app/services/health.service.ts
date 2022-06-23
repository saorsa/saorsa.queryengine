import { Injectable } from '@angular/core';
import { Observable } from "rxjs";
import { ApiService } from "./api.service";
import { ApiHealthResult } from "../model/api.model";


@Injectable({
  providedIn: 'root'
})
export class HealthService extends ApiService {

  public getHealth(): Observable<ApiHealthResult> {
    return this.get<ApiHealthResult>('health');
  }
}
