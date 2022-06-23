import { Injectable } from '@angular/core';
import { ApiService } from "./api.service";
import { Observable } from "rxjs";
import {
  ApiTypeDefinitionSingleResult,
  ApiTypeDefinitionsResult
} from "../model/api.model";


@Injectable({
  providedIn: 'root'
})
export class MetaService extends ApiService {

  public getCachedTypeDefinitions(): Observable<ApiTypeDefinitionsResult> {
    return this.get<ApiTypeDefinitionsResult>('meta/cached');
  }

  public getCachedTypeDefinition(typeName: string): Observable<ApiTypeDefinitionSingleResult> {
    return this.get<ApiTypeDefinitionSingleResult>(`meta/cached/${typeName}`);
  }
}
