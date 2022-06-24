import {
  Component, OnInit
} from '@angular/core';
import { MetaService } from "../../services/meta.service";
import {
  catchError, Observable, of
} from "rxjs";
import {
  ApiTypeDefinitionsResult
} from "../../model/api.model";
import {
  TypeDefinition
} from "../../model/query-engine.model";
import {
  ActivatedRoute, Router
} from "@angular/router";


@Component({
  selector: 'app-type-definition-select',
  templateUrl: './type-definition-select.component.html',
  styleUrls: ['./type-definition-select.component.sass']
})
export class TypeDefinitionSelectComponent implements OnInit {

  /** Stores the result with all type definitions, cached by the server. */
  apiResult?: ApiTypeDefinitionsResult | null;

  /** Stores the last known error when communicating with the server. */
  error?: any;

  /** Stores indication, if the component is loading data from the server. */
  loading = false;

  /** Stores the name of the currently active type definition from the activated route. */
  typeNameFromRoute?: string;

  /** Stores the currently selected type definition. */
  selectedTypDef?: TypeDefinition;

  constructor(
    private api: MetaService,
    private router: Router,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.activatedRoute.firstChild?.params?.subscribe(p => {
      this.typeNameFromRoute = p['type'];
    });
    this.loadTypeDefinitions();
  }

  onChangeSelectedType(event: any): void {
    const val: TypeDefinition = event?.source?.value;
    this.selectedTypDef = val;
    this.router.navigate([val?.name]).finally(() =>{});
  }

  protected loadTypeDefinitions(): Observable<ApiTypeDefinitionsResult> {
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
