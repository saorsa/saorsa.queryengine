import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import {
  ControlValueAccessor, FormArray, FormBuilder, FormControl, FormGroup, NG_VALUE_ACCESSOR, ValidationErrors, Validators
} from "@angular/forms";
import {
  FilterDescriptor, FilterOperatorType, PropertyFilter, QueryableTypeDescriptor
} from "../../model/query-engine.model";
import {FormsHelperService} from "../../services/forms-helper.service";


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

  @Input() typeDefinition?: QueryableTypeDescriptor;
  @Input() propertyFilter?: PropertyFilter;
  @Input() formGroup?: FormGroup;
  @Output() onChanges = new EventEmitter<any>();
  @Output() onValueChange = new EventEmitter<PropertyFilter>();
  @Output() onValidationError = new EventEmitter<ValidationErrors>();

  get safeFormGroupInstance(): FormGroup {
    return this.formGroup ?? this.internalFormGroup;
  }

  protected internalFormGroup: FormGroup;
  selectedProperty?: QueryableTypeDescriptor;
  selectedFilterDefinition?: FilterDescriptor;
  selectedFilterType?: FilterOperatorType;
  isDisabled = false;
  touched = false;

  onChange = (arg?: any) => {
    this.changeDetectorRef.detectChanges();
    const validationErrors = this.formsHelper.getErrorsInDepth(this.safeFormGroupInstance);
    console.warn('FILTERS ON CHANGES', this.safeFormGroupInstance.value, 'args', arg);
    this.onChanges.emit(this.safeFormGroupInstance.value);

    if (this.safeFormGroupInstance.valid) {
      const propFilter = this.safeFormGroupInstance.value as PropertyFilter;
      console.warn('FILTERS VALIDATION SUCCESS', propFilter);
      this.onValueChange.emit(propFilter);
    }
    else if (validationErrors) {
      console.error('FILTERS VALIDATION ERROR', validationErrors);
      this.onValidationError.emit(validationErrors!);
    }
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
    private formsHelper: FormsHelperService,
  ) {
    this.internalFormGroup = this.buildInternalFormGroup();
  }

  writeValue(obj: any): void {
    console.warn('WRITE VALUE', obj);
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
    if (this.selectedProperty == null) {
      this.selectedProperty = this.typeDefinition?.properties?.find(
        p => p.name === this.nameControl.value
      );
      this.selectedFilterDefinition = this.selectedProperty?.allowedFilters?.find(
        f => f.operatorType === this.filterTypeControl.value
      );
    }
  }

  onPropertySelect(propertyName: string): void {
    this.selectedProperty = this.typeDefinition?.properties?.find(p => p.name == propertyName);
    this.selectedFilterDefinition = this.selectedProperty?.allowedFilters?.length ?
      this.selectedProperty.allowedFilters[0] :
      undefined;
    this.filterTypeControl.setValue(this.selectedFilterDefinition?.operatorType);
    this.onChange(this.safeFormGroupInstance.value);
  }

  onFilterTypeSelect(filterType: FilterOperatorType): void {
    this.selectedFilterType = filterType;
    this.selectedFilterDefinition = this.selectedProperty?.allowedFilters?.find(f => f.operatorType == filterType);
    console.warn('XXXX', this.selectedFilterDefinition, filterType)
    this.filterTypeControl.setValue(this.selectedFilterDefinition?.operatorType);
    this.onChange(this.safeFormGroupInstance.value);
  }

  onArgumentChanges(args: any) : void {
    console.warn('onArgumentChanges', args);
    //this.onChange(this.safeFormGroupInstance.value);
  }

  onArgumentsArrayValueChange(args: any[]) {
    console.warn('onArgumentsArrayValueChange', args);
    this.onChange(this.safeFormGroupInstance.value);
  }

  onArgumentsArrayValidationErrors(errors: ValidationErrors) {
    console.warn('onArgumentsArrayValidationErrors', errors);
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
