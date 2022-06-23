import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {BinaryOperator, FilterType, PropertyFilterBlock, TypeDefinition} from "../../model/query-engine.model";
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
  @Input() parentCondition?: BinaryOperator;
  @Input() index = 0;
  @Input() filterExpression = new Subject<PropertyFilterBlock>();
  @Input() depth = 0;
  @Input() formGroup?: FormGroup = this.buildInternalFormGroup('And');
  @Output() onRemoveBlockItem = new EventEmitter<[PropertyFilterBlock, number]>();

  isDisabled = false;
  touched = false;
  onChange = (arg: any) => {
    console.warn('onChange', arg);
  };
  onTouched = () => {};

  expression?: PropertyFilterBlock;
  isHoveringConditionBlock = false;

  protected internalFormGroup: FormGroup = this.buildInternalFormGroup('And');

  get safeFormGroupInstance(): FormGroup {
    return this.formGroup ?? this.internalFormGroup;
  }

  get condition(): BinaryOperator {
    return this.conditionControl.value as BinaryOperator;
  }

  get isRootBlock(): boolean {
    return this.depth === 0;
  }

  get isSeparateConditionBlockLeaf(): boolean {
    return this.parentCondition != undefined
      && this.parentCondition !== this.condition;
  }

  get isSameConditionBlockLeaf(): boolean {
    return this.parentCondition != undefined
      && this.parentCondition === this.condition;
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
    const argumentControls = this.buildInternalFormGroups(blocks);
    console.warn('block controls', argumentControls.length, argumentControls);
    argumentControls.forEach(ac => {

      console.warn('pushing child with value', ac.value.first);
      this.othersFormArray.push(ac);
    });
  }

  addSingleOtherBlockControl(): void {
    const argumentControl = this.buildInternalFormGroup(this.condition);
    this.othersFormArray.push(argumentControl);
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
  }

  readonly binaryOpsChoices: BinaryOperator[] = [ 'And', 'Or'];

  constructor(
    private readonly formBuilder: FormBuilder,
  ){ }

  ngOnInit(): void {
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

  protected buildInternalFormGroup(condition: BinaryOperator): FormGroup {
    const filterType: FilterType =
      'IS_NOT_NULL';

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

  protected buildInternalFormGroups(blocks: PropertyFilterBlock[]): FormGroup[] {
    return blocks.map(otherItem =>
      this.formBuilder.group({
        first: this.formBuilder.group({
          name: [otherItem.first.name, Validators.required],
          filterType: [otherItem.first.filterType, Validators.required],
          arguments: this.formBuilder.array(otherItem.first.arguments || [])
        }),
        condition: [otherItem.condition],
        others: this.formBuilder.array([])
      })
    );
  }
}
