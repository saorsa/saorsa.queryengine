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

  @Input() formGroup?: FormGroup = this.buildInternalFormGroup();

  get safeFormGroupInstance(): FormGroup {
    return this.formGroup ?? this.internalFormGroup;
  }

  protected internalFormGroup: FormGroup = this.buildInternalFormGroup();

  get firstFormGroup(): FormGroup {
    return this.safeFormGroupInstance.get('first') as FormGroup;
  }

  get conditionControl(): FormControl {
    const result = this.safeFormGroupInstance.get('condition') as FormControl;
    return result;
  }

  get othersFormArray(): FormArray<FormGroup> {
    return this.safeFormGroupInstance.controls["others"] as FormArray;
  }

  addOthersBlockControl(): void {
    const argumentControl = this.buildInternalFormGroup();

    console.warn('adding child control', argumentControl);
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
    this.buildInternalFormGroup();
  }
  writeValue(obj: any): void {
    console.warn('WRITING BLOCK value', obj);
    if (obj) {
      this.firstFormGroup.setValue(obj.first);
      this.conditionControl.setValue(obj.condition);
      this.othersFormArray.setValue(obj.arguments);
    }
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

  protected buildInternalFormGroup(): FormGroup {
    const filterType: FilterType =
      'IS_NOT_NULL';

    const firstProperty = this.typeDefinition?.properties != null &&
      this.typeDefinition.properties.length > 0

      ?
      this.typeDefinition.properties[0].name :
      null;

    return this.formBuilder.group({
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
