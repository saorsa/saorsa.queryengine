import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output
} from '@angular/core';
import {
  LogicalOperator,
  FilterOperatorType, PropertyFilter,
  PropertyFilterBlock,
  QueryableTypeDescriptor
} from "../../model/query-engine.model";
import {
  ControlValueAccessor,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validators
} from "@angular/forms";
import {FormsHelperService} from "../../services/forms-helper.service";
import {Subject} from "rxjs";


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

  @Input() typeDefinition?: QueryableTypeDescriptor;
  @Input() parentCondition?: LogicalOperator;
  @Input() initialValue?: PropertyFilterBlock;
  @Input() index = 0;
  @Input() depth = 0;
  @Input() formGroup?: FormGroup = this.buildFormGroupFromState('And');
  @Output() onChanges = new EventEmitter<any>();
  @Output() onValueChange = new EventEmitter<PropertyFilterBlock>();
  @Output() onValidationError = new EventEmitter<ValidationErrors>();
  @Output() onRemoveBlockItem = new EventEmitter<[PropertyFilterBlock, number]>();

  isDisabled = false;
  touched = false;
  onChange = (arg: any) => {
    console.warn('BLOCK ON CHANGE', arg);
    const validationErrors = this.formsHelper.getErrorsInDepth(this.safeFormGroupInstance);

    if (this.safeFormGroupInstance.valid) {
      const propFilterBlock = this.safeFormGroupInstance.value as PropertyFilterBlock;
      console.warn('BLOCK VALIDATION SUCCESS', propFilterBlock);
      this.onValueChange.emit(propFilterBlock);
    }
    else if (validationErrors) {
      console.error('BLOCK VALIDATION ERROR', validationErrors);
      this.onValidationError.emit(validationErrors!);
    }
  };
  onTouched = () => {};

  expression?: PropertyFilterBlock;
  isHoveringConditionBlock = false;

  protected internalFormGroup: FormGroup = this.buildFormGroupFromState('And');

  get safeFormGroupInstance(): FormGroup {
    return this.formGroup ?? this.internalFormGroup;
  }

  get condition(): LogicalOperator {
    return this.conditionControl.value as LogicalOperator;
  }

  get isRootBlock(): boolean {
    return this.depth === 0;
  }

  get isSeparateConditionBlockLeaf(): boolean {
    return this.parentCondition != undefined
      && this.parentCondition !== this.condition;
  }

  get hasChildBlocks(): boolean {
    return this.othersFormArray != null && this.othersFormArray.length > 0;
  }

  get firstFormGroup(): FormGroup {
    return this.safeFormGroupInstance.get('first') as FormGroup;
  }

  get conditionControl(): FormControl {
    return this.safeFormGroupInstance.get('condition') as FormControl;
  }

  get othersFormArray(): FormArray<FormGroup> {
    return this.safeFormGroupInstance.controls["others"] as FormArray;
  }

  createChildBlocks(blocks: PropertyFilterBlock[]): void {
    const argumentControls = this.buildFormGroups(blocks);
    console.warn('block controls', argumentControls.length, argumentControls);
    argumentControls.forEach(ac => {

      console.warn('pushing child with value', ac.value.first);
      this.othersFormArray.push(ac);
    });
    this.onChange(this.safeFormGroupInstance.value);
  }

  addSingleOtherBlockControl(): void {
    const argumentControl = this.buildFormGroupFromState(this.condition);
    this.othersFormArray.push(argumentControl);
    this.onChange(this.safeFormGroupInstance.value);
  }

  removeOthersBlockControl(blockFrom?: FormGroup): void {
    if (blockFrom) {
      const matchIndex = this.othersFormArray.controls.findIndex(c => c === blockFrom);
      if (matchIndex >= 0) {
        this.othersFormArray.removeAt(matchIndex);
      }
    }
    else {
      while (this.othersFormArray.length > 0){
        this.othersFormArray.removeAt(0);
      }
    }
    this.onChange(this.safeFormGroupInstance.value);
  }

  removeFirstControl(): void {
    if (this.parentCondition != null) {
      const blockValue = this.safeFormGroupInstance.value as PropertyFilterBlock;
      console.warn('FIRING REMOVE EVENT', blockValue, blockValue.others);
      this.onRemoveBlockItem.emit([blockValue, this.index]);
    }
  }

  handleRemoveBlockFirstItem(event: [PropertyFilterBlock, number]): void {
    const block = event[0];
    const index = event[1];
    if (block.others?.length) {
      console.warn('Generating other items from blocks', block.others);
      this.createChildBlocks(block.others);
    }

    if (this.othersFormArray.length > index) {
      console.warn('Removing other item at index', index)
      this.othersFormArray.removeAt(index);
    }

    this.onChange(this.safeFormGroupInstance.value);
  }

  readonly binaryOpsChoices: LogicalOperator[] = [ 'And', 'Or'];

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly formsHelper: FormsHelperService,
  ){ }

  ngOnInit(): void {
    if (this.initialValue) {
      console.warn('INITIALIZING', this.initialValue);
      this.internalFormGroup = this.buildFormGroup(this.initialValue);
      this.formGroup = this.buildFormGroup(this.initialValue);
    }
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
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void{
    this.isDisabled = isDisabled;
  }

  onFirstChanges(args: any): void {
    console.warn('onFirstChanges', this.safeFormGroupInstance.valid, args);
    //this.onChange(this.safeFormGroupInstance.value);
  }

  onFirstValueChange(arg: PropertyFilter): void {
    console.warn('onFirstValueChange', arg);
    this.onChange(this.safeFormGroupInstance.value);
  }

  onFirstValidationErrors(errors: ValidationErrors): void {
    console.warn('onFirstValidationErrors', errors);
    this.onChange(this.safeFormGroupInstance.value);
  }

  onOthersBlockChanges(args: any): void {
    console.warn('onOthersBlockChanges', args);
    this.onChange(this.safeFormGroupInstance.value);
  }

  onOthersBlockValidationErrors(errors: ValidationErrors): void {
    console.warn('onOthersBlockValidationErrors', errors);
    this.onChange(this.safeFormGroupInstance.value);
  }

  onOthersBlockValueChange(arg: PropertyFilterBlock): void {
    console.warn('onOthersBlockValueChange', arg);
    this.onChange(this.safeFormGroupInstance.value);
  }

  protected buildFormGroupFromState(condition: LogicalOperator): FormGroup {
    const filterType: FilterOperatorType =
      'IsNotNull';

    const firstProperty =
      this.typeDefinition?.properties != null && this.typeDefinition.properties.length > 0
        ? this.typeDefinition.properties[0].name
        : null;

    return this.formBuilder.group({
      first: this.formBuilder.group({
        name: [firstProperty, Validators.required],
        filterType: [filterType, Validators.required],
        arguments: this.formBuilder.array([])
      }),
      condition: [ condition ],
      others: this.formBuilder.array([])
    });
  }

  protected buildFormGroups(blocks: PropertyFilterBlock[]): FormGroup[] {
    return blocks.map(otherItem => this.buildFormGroup(otherItem));
  }

  protected buildFormGroup(block: PropertyFilterBlock): FormGroup {
    return this.formBuilder.group({
      first: this.formBuilder.group({
        name: [block.first.name, Validators.required],
        filterType: [block.first.filterType, Validators.required],
        arguments: this.formBuilder.array(block.first.arguments || [])
      }),
      condition: [block.condition],
      others: this.formBuilder.array(
        this.buildFormGroups(block.others ?? [])
      )
    });
  }
}
