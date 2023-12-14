import { Injectable } from '@angular/core';
import { ApiService } from "./api.service";
import { Observable } from "rxjs";
import {
  ApiQueryableTypeSingleResult,
  ApiQueryableTypeListResult
} from "../model/api.model";


@Injectable({
  providedIn: 'root'
})
export class MetaService extends ApiService {

  public getCachedTypeDefinitions(): Observable<ApiQueryableTypeListResult> {
    return this.get<ApiQueryableTypeListResult>('meta/cached');
  }

  public getCachedTypeDefinition(typeName: string): Observable<ApiQueryableTypeSingleResult> {
    return this.get<ApiQueryableTypeSingleResult>(`meta/cached/${typeName}`);
  }
}
