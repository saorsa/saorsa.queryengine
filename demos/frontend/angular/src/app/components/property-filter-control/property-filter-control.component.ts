import {
  AfterViewChecked,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges
} from '@angular/core';
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
      multi: true,
      useExisting: PropertyFilterControlComponent
    }
  ]
})
export class PropertyFilterControlComponent implements OnInit, ControlValueAccessor {

  @Input() typeDefinition?: TypeDefinition;
  @Input() propertyFilter?: PropertyFilter;
  @Input() formGroup?: FormGroup;
  @Output() changes = new EventEmitter<any>();

  get safeFormGroupInstance(): FormGroup {
    return this.formGroup ?? this.internalFormGroup;
  }

  protected internalFormGroup: FormGroup;
  selectedProperty?: TypeDefinition;
  selectedFilterDefinition?: FilterDefinition;
  selectedFilterType?: FilterType;
  isDisabled = false;
  touched = false;
  onChange = (arg?: any) => {
    this.changes.emit(arg);
    this.changeDetectorRef.detectChanges();
  };
  onTouched = () => {
  };

  get nameControl(): FormControl {
    const nameControl = this.safeFormGroupInstance.controls['name'];
    if (nameControl == null) {
      throw new Error(
        "The property filter control requires a form group with a 'name' " +
        "form control."
      )
    }
    return nameControl as FormControl;
  }

  get filterTypeControl(): FormControl {
    const filterTypeControl = this.safeFormGroupInstance.controls['filterType'];
    if (filterTypeControl == null) {
      throw new Error(
        "The property filter control requires a form group with a 'filterType' " +
        "form control."
      )
    }
    return filterTypeControl as FormControl;
  }

  get argumentsControl(): FormArray {
    const argumentsFormArray = this.safeFormGroupInstance.controls['arguments'];
    if (argumentsFormArray == null) {
      throw new Error(
        "The property filter control requires a form group with a 'arguments' " +
        "form array."
      )
    }
    return argumentsFormArray as FormArray;
  }

  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    private formBuilder: FormBuilder,
  ) {
    this.internalFormGroup = this.buildInternalFormGroup();
  }

  writeValue(obj: any): void {
    const filter = obj as PropertyFilter;
    if (filter) {
      this.nameControl.setValue(filter.name);
      this.filterTypeControl.setValue(filter.filterType);
      this.argumentsControl.setValue(filter.arguments);
      this.changeDetectorRef.detectChanges();
      /**
      this.selectedProperty = this.typeDefinition?.properties?.find(
        p => p.name === filter.name
      );*/
    }
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
  }

  onPropertySelect(propertyName: string): void {
    this.selectedProperty = this.typeDefinition?.properties?.find(p => p.name == propertyName);
    this.onChange(this.safeFormGroupInstance.value);
  }

  onFilterTypeSelect(filterType: FilterType): void {
    this.selectedFilterType = filterType;
    this.selectedFilterDefinition = this.selectedProperty?.allowedFilters?.find(f => f.filterType == filterType);
    this.filterTypeControl.setValue(this.selectedFilterDefinition?.filterType);
    this.onChange(this.safeFormGroupInstance.value);
  }

  onArgumentsChange(args?: any) : void {
    this.onChange(this.safeFormGroupInstance.value);
  }

  protected markAsTouched() {
    if (!this.touched) {
      this.onTouched();
      this.touched = true;
    }
  }

  private buildInternalFormGroup(): FormGroup {
    const defaultFilterType = this.propertyFilter?.filterType || 'IS_NOT_NULL';
    return this.formBuilder.group({
      name: [this.propertyFilter?.name, Validators.required],
      filterType: [defaultFilterType, Validators.required],
      arguments: this.formBuilder.array(this.propertyFilter?.arguments || [])
    });
  }
}
