import { Component, Input, OnInit } from '@angular/core';
import {
  ControlValueAccessor, FormArray, FormBuilder, FormControl, FormGroup, NG_VALUE_ACCESSOR, Validators
} from "@angular/forms";
import {
  FilterDefinition, FilterType, PropertyFilter, TypeDefinition
} from "../../model/query-engine.model";


@Component({
  selector: 'app-property-filter-control',
  templateUrl: './property-filter-control.component.html',
  styleUrls: ['./property-filter-control.component.sass'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi:true,
      useExisting: PropertyFilterControlComponent
    }
  ]
})
export class PropertyFilterControlComponent implements OnInit, ControlValueAccessor {

  @Input() typeDefinition?: TypeDefinition;
  @Input() propertyFilter?: PropertyFilter;
  formGroup?: FormGroup;
  selectedProperty?: TypeDefinition;
  selectedFilterDefinition?: FilterDefinition;
  selectedFilterType?: FilterType;
  isDisabled = false;
  touched = false;
  onChange = (arg?: any) => {
    console.warn('onChange', arg);
  };
  onTouched = () => {};

  get nameControl(): FormControl {
    return this.formGroup?.get('name') as FormControl;
  }

  get filterTypeControl(): FormControl {
    return this.formGroup?.get('filterType') as FormControl;
  }

  get argumentsControl(): FormArray {
    return this.formGroup?.get('arguments') as FormArray;
  }

  constructor(
    private formBuilder: FormBuilder,
  ){
  }

  writeValue(obj: any): void {
    console.warn('writing value', obj);
    const filter = obj as PropertyFilter;
    if (filter) {
      this.nameControl.setValue(filter.name);
      this.filterTypeControl.setValue(filter.filterType);
      this.argumentsControl.setValue(filter.arguments);
      console.warn('done writing');
    }
  }

  registerOnChange(fn: any): void {
    console.log('Registering parent change tracking');
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void{
    this.isDisabled = isDisabled;
  }

  ngOnInit(): void {
    this.buildForm();
  }

  onPropertySelect(propertyName: string): void {
    this.selectedProperty = this.typeDefinition?.properties?.find(p => p.name == propertyName);
    console.warn('selected property', this.selectedProperty);
    this.onChange(this.formGroup?.value);
  }

  onFilterTypeSelect(filterType: FilterType): void {
    this.selectedFilterType = filterType;
    this.selectedFilterDefinition = this.selectedProperty?.allowedFilters?.find(f => f.filterType == filterType);
    this.filterTypeControl.setValue(this.selectedFilterDefinition?.filterType);
    console.warn('selected filter', this.selectedFilterDefinition);
    this.onChange(this.formGroup?.value);
  }

  private markAsTouched() {
    if (!this.touched) {
      this.onTouched();
      this.touched = true;
    }
  }

  private buildForm(): void {
    const IS_NOT_NULL: FilterType = 'IS_NOT_NULL';
    this.formGroup = this.formBuilder.group({
      name: [this.propertyFilter?.name, Validators.required],
      filterType: [this.propertyFilter?.filterType || IS_NOT_NULL, Validators.required],
      arguments: this.formBuilder.array(this.propertyFilter?.arguments || [])
    });
  }
}
