import { Component, OnInit } from '@angular/core';
import { TypeDefinition } from "../../model/query-engine.model";
import { ActivatedRoute } from "@angular/router";
import { MetaService } from "../../services/meta.service";
import { catchError, Observable, of } from "rxjs";
import { ApiTypeDefinitionSingleResult } from "../../model/api.model";

@Component({
  selector: 'app-type-definition-view',
  templateUrl: './type-definition-view.component.html',
  styleUrls: ['./type-definition-view.component.sass']
})
export class TypeDefinitionViewComponent implements OnInit {

  apiResult?: ApiTypeDefinitionSingleResult | null;
  error?: any;
  loading = false;
  typeDefinition?: TypeDefinition;

  constructor(
    private activatedRoute: ActivatedRoute,
    private api: MetaService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe( p => {
      console.warn('param type = ', p['type']);
      this.loadCachedTypeDef(p['type']);
    });
  }

  private loadCachedTypeDef(typeName: string): Observable<ApiTypeDefinitionSingleResult> {
    this.loading = true;
    this.error = null;
    this.apiResult = null;
    const result = this.api.getCachedTypeDefinition(typeName);
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
