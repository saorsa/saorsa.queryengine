import {Component, Input, OnInit, Output} from '@angular/core';
import {
  BinaryOperator,
  FilterDefinition,
  FilterType,
  PropertyFilterBlock,
  TypeDefinition
} from "../../model/query-engine.model";
import {Subject} from "rxjs";
import {FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {MatSelect} from "@angular/material/select";

@Component({
  selector: 'app-property-filter-control-block-view',
  templateUrl: './property-filter-block-view.component.html',
  styleUrls: ['./property-filter-block-view.component.sass']
})
export class PropertyFilterBlockViewComponent implements OnInit {

  @Input() typeDefinition?: TypeDefinition;
  @Input() filterExpression = new Subject<PropertyFilterBlock>();

  expression?: PropertyFilterBlock;
  selectedProperty?: TypeDefinition;
  selectedFilterType?: FilterType;
  selectedFilterDefinition?: FilterDefinition;

  formGroup?: FormGroup;

  get firstControl(): FormControl {
    return this.formGroup?.get('first') as FormControl;
  }


  readonly binaryOpsChoices: BinaryOperator[] = [ 'And', 'Or'];

  get hasFilters(): boolean{
    return this.expression?.first != null;
  }

  constructor(
    private readonly formBuilder: FormBuilder,
  ){ }

  ngOnInit(): void {
    this.buildForm();
  }

  private buildForm(): void {
    this.formGroup = this.formBuilder.group({
      first: [null, Validators.required],
      condition: [],
      others: this.formBuilder.array([])
    });
  }
}


