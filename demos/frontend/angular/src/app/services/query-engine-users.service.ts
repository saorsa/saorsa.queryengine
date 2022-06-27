import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {ApiTypeDefinitionsResult} from "../model/api.model";
import {ApiService} from "./api.service";
import {PropertyFilterBlock} from "../model/query-engine.model";

@Injectable({
  providedIn: 'root'
})
export class QueryEngineUsersService extends ApiService{

  public filterUsers(
    expression: PropertyFilterBlock,
    pageIndex = 0,
    pageSize = 10): Observable<any> {
    const pageRequest = {
      pageIndex,
      pageSize,
      filterExpression: expression,
    };
    return this.post<any, any>('database/users/query', pageRequest);
  }
}

