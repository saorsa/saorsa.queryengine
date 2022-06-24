import {
  Component, OnInit
} from '@angular/core';
import {
  PropertyFilterBlock, TypeDefinition
} from "../../model/query-engine.model";
import {
  ActivatedRoute, Router
} from "@angular/router";
import { MetaService } from "../../services/meta.service";
import {
  catchError, Observable, of, Subject
} from "rxjs";
import { ApiTypeDefinitionSingleResult } from "../../model/api.model";


@Component({
  selector: 'app-type-definition-view',
  templateUrl: './type-definition-view.component.html',
  styleUrls: ['./type-definition-view.component.sass']
})
export class TypeDefinitionViewComponent implements OnInit {

  protected typeDefinitionApiResult?: ApiTypeDefinitionSingleResult | null;
  protected typeName?: string | null;
  readonly expression$ = new Subject<PropertyFilterBlock>();

  /** The filter expression to be used upon the initialization of the component. Optional. */
  initialFilterBlock?: PropertyFilterBlock;

  /** The currently active filter block. Defaults to NULL, before one is constructed without errors. */
  currentFilterBlock?: PropertyFilterBlock;

  /** Stores the last known error when communicating with the server. */
  error?: any;

  /** Stores indication, if the component is loading data from the server. */
  loading = false;

  /** Gets the loaded type definition. Returns null, if the definition cannot be loaded */
  get typeDefinition(): TypeDefinition | null {
    return this.typeDefinitionApiResult?.result || null;
  }

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private api: MetaService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe( routeParams => {
      const typeParam = routeParams['type'];
      if (this.typeName && typeParam !== this.typeName) {
        this.initialFilterBlock = undefined;
        this.resetExpressionInQueryString();
        this.typeName = typeParam;
      }
      this.typeName = typeParam;
      this.loadTypeDefinition(typeParam);
    });
    this.parseExpressionFromQueryString();
  }

  onExpressionValueChange(expression: PropertyFilterBlock): void {
    console.debug(
      `[${this.constructor.name}] Expression tree has changed:`, expression,
      'Previous expression:', this.currentFilterBlock);
    const expressionJson = JSON.stringify(expression);
    this.router.navigate([],{
      relativeTo: this.activatedRoute,
      queryParams: {
        expression: encodeURIComponent(expressionJson),
      },
    })
    .catch(err => {
      console.error(
        `[${this.constructor.name}] Error building expression in query string`, err);
    })
    .finally(() => {
      this.currentFilterBlock = expression;
    });
  }

  protected resetExpressionInQueryString(): void {
    this.router.navigate([],{
      relativeTo: this.activatedRoute,
      queryParams: {},
      queryParamsHandling: ''
    })
      .catch(err => {
        console.error(
          `[${this.constructor.name}] Error resetting expression in query string`, err);
      })
      .finally(() => {
      });
  }

  protected parseExpressionFromQueryString(): void {
    this.activatedRoute.queryParams
      .subscribe(params => {
        const expressionString = params['expression'];
        if (expressionString) {
          this.initialFilterBlock = undefined;
          const decoded = decodeURIComponent(expressionString);
          if (decoded) {
            const parsed = JSON.parse(decoded) as PropertyFilterBlock;
            this.initialFilterBlock = parsed;
            this.expression$.next(parsed);
            console.debug(
              `[${this.constructor.name}] Parsed expression from query string`, parsed);
          }
        }
      });
  }

  protected loadTypeDefinition(typeName: string): Observable<ApiTypeDefinitionSingleResult> {
    this.loading = true;
    this.error = null;
    this.typeDefinitionApiResult = null;
    const result = this.api.getCachedTypeDefinition(typeName);
    result
      .pipe(catchError(err => {
        this.error = err;
        return of(null)
      }))
      .subscribe(result => {
        this.typeDefinitionApiResult = result;
        this.loading = false;
      });
    return result;
  }
}
