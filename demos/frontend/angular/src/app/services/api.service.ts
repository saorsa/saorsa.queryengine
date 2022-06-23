import {
  Injectable
} from '@angular/core';
import { environment } from "../../environments/environment";
import {
  Observable
} from "rxjs";
import {
  HttpClient,
  HttpParams
} from "@angular/common/http";


@Injectable({
  providedIn: 'root'
})
export class ApiService {

  readonly baseUrl: string = environment.backend;

  constructor(
    private http: HttpClient,
  ) { }

  get<T>(path: string, params: any = null): Observable<T> {
    const fullPath = `${this.baseUrl}${path}`;
    const httpParams = ApiService.toQueryParams(params);
    return this.http.get<T>(fullPath, { params: httpParams });
  }

  private static toQueryParams(obj: any): HttpParams{
    let params = new HttpParams();
    for (const key in obj) {
      if (obj.hasOwnProperty(key)) {
        params = params.append(encodeURIComponent(key), encodeURIComponent(obj[key]));
      }
    }
    return params;
  }
}
