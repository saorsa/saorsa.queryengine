import { Injectable } from '@angular/core';
import { PropertyFilterBlock } from "../model/query-engine.model";
import { Observable } from "rxjs";
import { ApiService } from "./api.service";

@Injectable({
  providedIn: 'root'
})
export class QueryEngineCategoriesService extends ApiService {

  public filterCategories(
    expression: PropertyFilterBlock,
    pageIndex = 0,
    pageSize = 10): Observable<any> {
    const pageRequest = {
      pageIndex,
      pageSize,
      filterExpression: expression,
    };
    return this.post<any, any>('database/categories/query', pageRequest);
  }
}
