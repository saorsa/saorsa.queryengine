import { Injectable } from '@angular/core';
import {ApiService} from "./api.service";
import {Observable} from "rxjs";
import {ApiHealthResult} from "../model/api.model";

@Injectable({
  providedIn: 'root'
})
export class DataGeneratorService extends ApiService {

  public fillData(): Observable<any> {
    return this.get<any>('database/data/fill');
  }
}
