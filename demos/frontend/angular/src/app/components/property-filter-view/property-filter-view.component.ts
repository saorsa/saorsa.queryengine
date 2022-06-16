import { Component, Input, OnInit } from '@angular/core';
import {
  ControlValueAccessor, FormBuilder, FormControl, FormGroup, NG_VALUE_ACCESSOR, Validators
} from "@angular/forms";
import {
  FilterDefinition, FilterType, PropertyFilter, TypeDefinition
} from "../../model/query-engine.model";


@Component({
  selector: 'app-property-filter-view-view',
  templateUrl: './property-filter-view.component.html',
  styleUrls: ['./property-filter-view.component.sass'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi:true,
      useExisting: PropertyFilterViewComponent
    }
  ]
})
export class PropertyFilterViewComponent implements OnInit, ControlValueAccessor {

  @Input() typeDefinition?: TypeDefinition;
  @Input() propertyFilter?: PropertyFilter;
  formGroup?: FormGroup;
  selectedProperty?: TypeDefinition;
  selectedFilterDefinition?: FilterDefinition;
  selectedFilterType?: FilterType;
  isDisabled = false;
  touched = false;
  onChange = () => {
    console.warn('onChange');
  };
  onTouched = () => {};

  get nameControl(): FormControl {
    return this.formGroup?.get('name') as FormControl;
  }

  get filterTypeControl(): FormControl {
    return this.formGroup?.get('filterType') as FormControl;
  }

  constructor(
    private formBuilder: FormBuilder,
  ){
  }

  writeValue(obj: any): void {
    console.warn('writing value', obj);
  }

  registerOnChange(fn: any): void {
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
    console.warn('1');
    this.selectedProperty = this.typeDefinition?.properties?.find(p => p.name == propertyName);
    console.warn('selected', this.selectedProperty);
  }

  onFilterTypeSelect(filterType: FilterType): void {
    this.selectedFilterType = filterType;
    this.selectedFilterDefinition = this.selectedProperty?.allowedFilters?.find(f => f.filterType == filterType);
    this.filterTypeControl.setValue(this.selectedFilterDefinition?.filterType);
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
      arguments: this.formBuilder.group(this.propertyFilter?.arguments || [])
    });
  }
}
