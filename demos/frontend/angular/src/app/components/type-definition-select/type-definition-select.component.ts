import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {MetaService} from "../../services/meta.service";
import {catchError, Observable, of} from "rxjs";
import {ApiTypeDefinitionsResult} from "../../model/api.model";
import {TypeDefinition} from "../../model/query-engine.model";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-type-definition-select',
  templateUrl: './type-definition-select.component.html',
  styleUrls: ['./type-definition-select.component.sass']
})
export class TypeDefinitionSelectComponent implements OnInit {

  @Output() onTypeDefinitionSelect = new EventEmitter<TypeDefinition>();

  apiResult?: ApiTypeDefinitionsResult | null;
  error?: any;
  loading = false;
  typeNameFromRoute?: string;
  selectedTypDef?: TypeDefinition;

  constructor(
    private api: MetaService,
    private router: Router,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.activatedRoute.firstChild?.params?.subscribe(p => {
      this.typeNameFromRoute = p['type'];
    });
    this.loadCachedTypeDefs();
  }

  onChange(event: any): void {
    const val: TypeDefinition = event?.source?.value;
    this.selectedTypDef = val;
    this.router.navigate([val?.name]).finally(() =>{});
  }

  private loadCachedTypeDefs(): Observable<ApiTypeDefinitionsResult> {
    this.loading = true;
    this.error = null;
    this.apiResult = null;
    const result = this.api.getCachedTypeDefinitions();
    result
      .pipe(catchError(err => {
        this.error = err;
        return of(null)
      }))
      .subscribe(result => {
        this.apiResult = result;
        this.selectedTypDef = result?.result?.find(x => x.name == this.typeNameFromRoute);
        this.loading = false;
      });
    return result;
  }
}
