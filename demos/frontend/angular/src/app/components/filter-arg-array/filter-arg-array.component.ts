import {Component, Input, OnInit, Output} from '@angular/core';
import {FilterType, PropertyFilter, TypeDefinition} from "../../model/query-engine.model";
import {Subject} from "rxjs";
import {Form, FormArray, FormBuilder, FormControl, FormGroup} from "@angular/forms";

@Component({
  selector: 'app-filter-arg-array',
  templateUrl: './filter-arg-array.component.html',
  styleUrls: ['./filter-arg-array.component.sass']
})
export class FilterArgArrayComponent implements OnInit {

  @Input() type?: TypeDefinition | null;
  @Input() property?: TypeDefinition | null;
  @Input() filterType?: FilterType | null;
  @Output() propertyFilter$ = new Subject<PropertyFilter>();

  readonly args: object[] = [];
  form?: FormGroup;


  get isMultiInput(): boolean {
    return this.filterType === 'SEQUENCE';
  }

  constructor(
    readonly formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    this.buildForm();
  }

  private buildForm(): void {
    this.form = this.formBuilder.group({
      args: this.formBuilder.array([])
    });
  }
}
