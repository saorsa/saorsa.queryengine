import { Component, OnInit } from '@angular/core';
import { DataGeneratorService } from "../../services/data-generator.service";
import {catchError, map, of, startWith} from "rxjs";
import {FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {Category, User, UserLogonType} from "../../model/api.model";
import {QueryEngineCategoriesService} from "../../services/query-engine-categories.service";

@Component({
  selector: 'app-data-generator',
  templateUrl: './data-generator.component.html',
  styleUrls: ['./data-generator.component.sass']
})
export class DataGeneratorComponent implements OnInit {

  readonly logonTypes: UserLogonType[] = [
    'Oidc',
    'Saml',
    'ActiveDirectory'];

  loading = false;
  error: any = null;
  result: any = null;
  formGroup: FormGroup;
  categories?: Category[];
  autocompleteCategories?: Category[];

  get idControl(): FormControl {
    return this.formGroup.get('id') as FormControl;
  }

  get usernameControl(): FormControl {
    return this.formGroup.get('username') as FormControl;
  }

  get passwordControl(): FormControl {
    return this.formGroup.get('password') as FormControl;
  }

  get logonTypeControl(): FormControl {
    return this.formGroup.get('latestLogonType') as FormControl;
  }

  get genderControl(): FormControl {
    return this.formGroup.get('gender') as FormControl;
  }

  get externalIdControl(): FormControl {
    return this.formGroup.get('externalId') as FormControl;
  }

  get categoryIdControl(): FormControl {
    return this.formGroup.get('categoryId') as FormControl;
  }

  get categoryIdSearchControl(): FormControl<string | Category> {
    return this.formGroup.get('categoryIdSearch') as FormControl<string | Category>;
  }


  constructor(
    readonly dataGenerator: DataGeneratorService,
    readonly formBuilder: FormBuilder,
    readonly queryCategoriesService: QueryEngineCategoriesService,
  ) {
    this.formGroup = this.formBuilder.group({
      id: new FormControl({ value: null, disabled: true} ),
      username: [`user-${this.generateGuid()}`, Validators.required],
      password: [],
      gender: [],
      externalId: [],
      latestLogonType: [],
      categoryId: [],
      categoryIdSearch: [],
    });
  }

  generateGuid = () =>
    "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
      var r = (Math.random() * 16) | 0,
        v = c == "x" ? r : (r & 0x3) | 0x8;
      return v.toString(16);
    });

  ngOnInit(): void {
    this.loadCategories();
    this.categoryIdSearchControl.valueChanges.pipe(
      startWith(''),
      map(value => {
        console.warn('value' ,value, typeof value);
        const name = typeof value === 'string' ? value : value?.name;
        return value;
      }),
    ).subscribe(r => {
      console.warn('r', r);
      if (typeof r === 'string'){
        this.loadCategories(r);
      }
    })
  }

  generateData(): void {
    this.error = null;
    this.loading = true;
    this.result = null;
    this.dataGenerator.fillData()
      .pipe(catchError(err => {
        this.error = err;
        return of(null)
      }))
      .subscribe(result => {
        this.result = result;
        this.loading = false;
      })
  }

  loadCategories(searchKey?: string): void {
    this.queryCategoriesService.filterCategories({
      first: searchKey?.length ? {
        name: 'Name',
        filterType: 'CONTAINS',
        arguments: [ searchKey ]
      } :  {
        name: 'Id',
        filterType: 'IS_NOT_NULL',
        arguments: []
      }
    })
      .pipe(catchError(err => {
        console.error(err);
        return of(null);
      }))
      .subscribe(result => {
        console.warn('res', result);
        this.categories = result.result;
      })
  }

  renderCategoryAutocomplete(category: Category): string {
    return category?.name ?? '';
  }

  optionSelected(ev: any): void {
    this.categoryIdControl.setValue(ev.option.value.id)
  }
}
