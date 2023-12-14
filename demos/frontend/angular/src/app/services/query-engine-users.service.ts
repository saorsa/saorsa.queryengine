import { Injectable } from '@angular/core';
import { Observable } from "rxjs";
import { ApiService } from "./api.service";
import {PropertyFilterBlock, QueryableTypeDescriptor} from "../model/query-engine.model";

@Injectable({
  providedIn: 'root'
})
export class QueryEngineUsersService extends ApiService {

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

  public filterEntities(
    typeDescriptor: QueryableTypeDescriptor,
    expression: PropertyFilterBlock,
    pageIndex = 0,
    pageSize = 10): Observable<any> {
    const entityName = typeDescriptor.name.toLowerCase()
    const pageRequest = {
      pageIndex,
      pageSize,
      filterExpression: expression,
    };
    return this.post<any, any>(`database/${entityName}s/query`, pageRequest);
  }
}

