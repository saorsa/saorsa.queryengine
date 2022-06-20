import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges} from '@angular/core';
import {FilterDefinition, TypeDefinition} from "../../model/query-engine.model";
import {
  AbstractControl,
  ControlValueAccessor,
  FormArray,
  FormBuilder, FormControl,
  FormGroup,
  NG_VALUE_ACCESSOR, Validators,
} from "@angular/forms";
import {QueryEngineTypeSystemService} from "../../services/query-engine-type-system.service";


@Component({
  selector: 'app-property-argument-array-control',
  templateUrl: './property-argument-array-control.component.html',
  styleUrls: ['./property-argument-array-control.sass'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: PropertyArgumentArrayControlComponent
    }
  ]
})
export class PropertyArgumentArrayControlComponent implements OnInit, OnChanges, ControlValueAccessor {

  @Input() type?: TypeDefinition | null;
  @Input() property?: TypeDefinition | null;
  @Input() filterDefinition?: FilterDefinition | null;
  @Input() formGroup?: FormGroup;
  @Output() changes = new EventEmitter<any>();

  get safeFormGroupInstance(): FormGroup {
    return this.formGroup ?? this.internalFormGroup;
  }

  get argumentsFormArray(): FormArray<FormControl> {
    const argumentsFormArray = this.safeFormGroupInstance.controls['arguments'];
    if (argumentsFormArray == null) {
      throw new Error(
        "The property argument array control requires a form group with an 'arguments' " +
        "form array."
      )
    }
    return argumentsFormArray as FormArray;
  }

  protected internalFormGroup: FormGroup;
  isDisabled = false;
  touched = false;
  minArgumentsCount = -1;
  maxArgumentCount?: number | null;
  onChange = (args?:any) => {
    this.changes.emit(args);
  };
  onTouched = () => {
  };

  readonly args: object[] = [];

  get requiresDynamicArray(): boolean {
    if (!this.filterDefinition) return false;
    return this.filterTypesService.expectsDynamicArguments(this.filterDefinition!);
  }

  constructor(
    readonly formBuilder: FormBuilder,
    readonly filterTypesService: QueryEngineTypeSystemService,
  ) {
    this.internalFormGroup = this.buildInternalFormGroup();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['filterDefinition']) {
      this.rebuildFormIfNeeded();
    }
  }

  writeValue(obj: any): void {
    console.error('property-arg-array writing value', obj);
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

  addArgumentControl(): void {
    const argumentControl = this.formBuilder.control(
      null, Validators.required
    );
    this.argumentsFormArray.push(argumentControl);
    this.onChange(this.argumentsFormArray.value);
  }

  get argumentControlInputType(): string {
    if (this.property) {
      return this.filterTypesService.getInputControlType(this.property!.type);
    }
    return "unknown"
  }

  removeArgumentControl(formArrayItem?: AbstractControl): void {
    if (formArrayItem) {
      const matchIndex = this.argumentsFormArray.controls.findIndex(c => c === formArrayItem);
      if (matchIndex >= 0) {
        this.argumentsFormArray.removeAt(matchIndex);
      }
    }
    else {
      const lastIndex = this.argumentsFormArray.length - 1;
      if (lastIndex >= 0) {
        this.argumentsFormArray.removeAt(lastIndex);
      }
    }
  }

  inputValueChange(_: any): void {
    this.onChange(this.argumentsFormArray.value);
  }

  protected markAsTouched() {
    if (!this.touched) {
      this.onTouched();
      this.touched = true;
    }
  }

  protected rebuildFormIfNeeded(): void {
    this.minArgumentsCount = -1;
    if (this.filterDefinition) {
      this.minArgumentsCount = this.filterTypesService.argumentsMinCount(this.filterDefinition);
      this.maxArgumentCount = this.filterTypesService.argumentsMaxCount(this.filterDefinition);
      let controlCount = this.argumentsFormArray.controls.length;
      if (this.minArgumentsCount > 0 && controlCount <= 0) {
        if (controlCount < this.minArgumentsCount) {
          const diff = this.minArgumentsCount - controlCount;
          for (let idx=0; idx < diff; idx++) {
            this.addArgumentControl();
            controlCount++;
          }
        }
      }

      if (this.maxArgumentCount != null){
         if (controlCount < this.maxArgumentCount) {
          const diff = this.maxArgumentCount - controlCount;
          for (let idx=0; idx < diff; idx++) {
            this.addArgumentControl();
            controlCount++;
          }
        }
        else if (controlCount > this.maxArgumentCount) {
          const diff = controlCount - this.maxArgumentCount;
          for (let idx=0; idx < diff; idx++) {
            this.removeArgumentControl();
            controlCount--;
          }
        }
      }
    }
  }

  protected buildInternalFormGroup(): FormGroup {
    return this.formBuilder.group({
      arguments: this.formBuilder.array([])
    });
  }
}
