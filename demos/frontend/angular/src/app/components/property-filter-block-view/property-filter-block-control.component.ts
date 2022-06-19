import {Component, Input, OnInit, Output} from '@angular/core';
import {
  BinaryOperator,
  FilterDefinition,
  FilterType,
  PropertyFilterBlock,
  TypeDefinition
} from "../../model/query-engine.model";
import {Subject} from "rxjs";
import {
  ControlValueAccessor,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  NG_VALUE_ACCESSOR,
  Validators
} from "@angular/forms";
import {MatSelect} from "@angular/material/select";

@Component({
  selector: 'app-property-filter-control-block-control',
  templateUrl: './property-filter-block-control.component.html',
  styleUrls: ['./property-filter-block-control.component.sass'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi:true,
      useExisting: PropertyFilterBlockControlComponent
    }
  ]
})
export class PropertyFilterBlockControlComponent implements OnInit, ControlValueAccessor {

  @Input() typeDefinition?: TypeDefinition;
  @Input() filterExpression = new Subject<PropertyFilterBlock>();
  @Input() depth = 0;
  isDisabled = false;
  touched = false;
  onChange = (arg: any) => {
    console.warn('onChange', arg);
  };
  onTouched = () => {};

  expression?: PropertyFilterBlock;
  selectedProperty?: TypeDefinition;
  selectedFilterType?: FilterType;
  selectedFilterDefinition?: FilterDefinition;

  formGroup?: FormGroup;

  get firstFormGroup(): FormGroup {
    return this.formGroup?.get('first') as FormGroup;
  }

  get conditionControl(): FormControl {
    const result = this.formGroup?.get('condition') as FormControl;
    console.warn('condition', result);
    return result;
  }

  get othersFormArray(): FormArray<FormGroup> {
    return this.formGroup?.controls["others"] as FormArray;
  }

  addOthersBlockControl(): void {
    const argumentControl = this.formBuilder.group({
      argument: [null, Validators.required]
    });
    this.othersFormArray.push(argumentControl);
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
  writeValue(obj: any): void {
    console.warn('WRITING BLOCK value', obj);
  }

  registerOnChange(fn: any): void {
    console.warn('here', fn);
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void{
    this.isDisabled = isDisabled;
  }

  private buildForm(): void {
    const filterType =
      'Is_NOT_NULL';

    const firstProperty = this.typeDefinition?.properties != null &&
      this.typeDefinition.properties.length > 0

      ?
      this.typeDefinition.properties[0].name :
      null;

    this.formGroup = this.formBuilder.group({
      first: this.formBuilder.group({
        name: [firstProperty, Validators.required],
        filterType: [filterType, Validators.required],
        arguments: this.formBuilder.array([])
      }),
      condition: ['And'],
      others: this.formBuilder.array([])
    });
  }
}


