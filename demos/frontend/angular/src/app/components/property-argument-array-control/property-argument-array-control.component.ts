import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges
} from '@angular/core';
import {
  FilterDefinition,
  TypeDefinition
} from "../../model/query-engine.model";
import {
  AbstractControl,
  ControlValueAccessor,
  FormArray,
  FormBuilder, FormControl,
  FormGroup,
  NG_VALUE_ACCESSOR, ValidationErrors, Validators,
} from "@angular/forms";
import { QueryEngineTypeSystemService } from "../../services/query-engine-type-system.service";
import { ReactiveFormsHelperService } from "../../services/reactive-forms-helper.service";


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
  @Output() onChanges = new EventEmitter<any>();
  @Output() onValueChange = new EventEmitter<any[]>();
  @Output() onValidationError = new EventEmitter<ValidationErrors>();

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
    console.warn('ARGS ON CHANGES', args);
    this.onChanges.emit(args);

    if (this.safeFormGroupInstance.valid) {
      console.warn('ARGS VALIDATION SUCCESS', args, this.argumentsFormArray.value);
      const value = this.argumentsFormArray.value;
      this.onValueChange.emit(value)
    }
    else {
      const validationErrors = this.formsHelper.getErrorsInDepth(this.safeFormGroupInstance);
      if (validationErrors) {
        console.error('ARGS VALIDATION ERROR', validationErrors);
        this.onValidationError.emit(validationErrors!);
      }
    }
  };

  onTouched = () => {};

  readonly args: object[] = [];

  get requiresDynamicArray(): boolean {
    if (!this.filterDefinition) return false;
    return this.filterTypesService.expectsDynamicArguments(this.filterDefinition!);
  }

  constructor(
    readonly formBuilder: FormBuilder,
    readonly filterTypesService: QueryEngineTypeSystemService,
    readonly formsHelper: ReactiveFormsHelperService,
  ) {
    this.internalFormGroup = this.buildInternalFormGroup();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['filterDefinition']) {
      console.warn('Rebuilding argument controls...')
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
    this.argumentsFormArray.controls.forEach(c => {
      isDisabled
        ? c.disable()
        : c.enable()
    });
  }

  getLabelText(index: number): string {
    let brief = 'Filter Value';
    if (this.filterDefinition?.filterType === 'RANGE') {
      brief = index == 0 ? 'Min Value' : 'Max Value';
    }
    if (this.filterDefinition?.filterType === 'SEQUENCE') {
      brief = `Argument [${index}]`;
    }
    return `${brief} (${this.argumentPropertyType})`;
  }

  ngOnInit(): void {}

  addArgumentControl(): void {
    const argumentControl = this.formBuilder.control(
      null, Validators.required
    );
    this.argumentsFormArray.push(argumentControl);
    this.onChange(this.argumentsFormArray.value);
  }

  get argumentControlInputType(): string {
    if (this.property) {
      return this.filterTypesService.getInputControlType(this.property.type);
    }
    return "unknown"
  }

  get argumentPropertyType(): string {
    if (this.property) {
      return this.property.type;
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
    this.onChange(this.argumentsFormArray.value);
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
