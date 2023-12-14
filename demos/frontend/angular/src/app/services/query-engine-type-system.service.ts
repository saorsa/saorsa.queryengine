import {
  Injectable
} from '@angular/core';
import {
  FilterDescriptor,
  FilterOperatorType,
  PropertyType,
  PropertyTypeStringMap
} from "../model/query-engine.model";


@Injectable({
  providedIn: 'root'
})
export class QueryEngineTypeSystemService {

  readonly PROPERTY_TYPE_TO_INPUT_TYPE: PropertyTypeStringMap = {
    byte : 'text',
    sbyte : 'text',
    boolean: 'checkbox',
    array : 'array',
    char : 'text',
    string: 'text',
    date : 'date',
    dateTime: 'dateTime',
    dateTimeOffset: 'dateTime',
    enum: 'enum',
    decimal: 'number',
    float: 'number',
    double: 'number',
    int16: 'number',
    integer: 'number',
    int64: 'number',
    uint16: 'number',
    uint32: 'number',
    uint64: 'number',
    object: 'object',
    uuid: 'text',
    time: 'text',
    timeSpan: 'text'
  }

  readonly SINGLE_ARGUMENT_FILTER_TYPES: FilterOperatorType[] = [
    'EqualTo',
    'NotEqualTo',
    'LessThan',
    'LessThanOrEqual',
    'GreaterThan',
    'GreaterThanOrEqual',
    'StringContains',
  ];

  readonly TWO_ARGUMENT_FILTER_TYPES: FilterOperatorType[] = [
    'ValueInRange',
  ];

  readonly DYNAMIC_ARGUMENT_FILTER_TYPES: FilterOperatorType[] = [
    'ValueInSequence',
  ];

  readonly NO_ARGUMENT_FILTER_TYPES: FilterOperatorType[] = [
    'IsNull',
    'IsNotNull',
    'CollectionIsEmpty',
    'CollectionIsNotEmpty'
  ];

  constructor() { }

  public getInputControlType(prop: PropertyType): string {
    const result = this.PROPERTY_TYPE_TO_INPUT_TYPE[prop];
    return result ?? 'unknown';
  }

  public expectsDynamicArguments(filter: FilterDescriptor | FilterOperatorType): boolean {
    const isFilterType = this.isFilterType(filter);
    if (isFilterType) {
      return this.DYNAMIC_ARGUMENT_FILTER_TYPES.includes(filter as FilterOperatorType);
    }
    return this.expectsDynamicArguments((filter as FilterDescriptor).operatorType);
  }

  public expectsTwoArguments(filter: FilterDescriptor | FilterOperatorType): boolean {
    const isFilterType = this.isFilterType(filter);
    if (isFilterType) {
      return this.TWO_ARGUMENT_FILTER_TYPES.includes(filter as FilterOperatorType);
    }
    return this.expectsTwoArguments((filter as FilterDescriptor).operatorType);
  }

  public expectsSingleArgument(filter: FilterDescriptor | FilterOperatorType): boolean {
    const isFilterType = this.isFilterType(filter);
    if (isFilterType) {
      return this.SINGLE_ARGUMENT_FILTER_TYPES.includes(filter as FilterOperatorType);
    }
    return this.expectsSingleArgument((filter as FilterDescriptor).operatorType);
  }

  public expectsNoArguments(filter: FilterDescriptor | FilterOperatorType): boolean {
    const isFilterType = this.isFilterType(filter);
    if (isFilterType) {
      return this.NO_ARGUMENT_FILTER_TYPES.includes(filter as FilterOperatorType);
    }
    return this.expectsNoArguments((filter as FilterDescriptor).operatorType);
  }

  public argumentsMinCount(filter: FilterDescriptor | FilterOperatorType): number {
    console.warn('------', filter)
    if (this.expectsTwoArguments(filter)) {
      console.warn('ATANAS argumentsMinCount 2', filter)
      return 2;
    }
    if (this.expectsSingleArgument(filter)) {
      console.warn('ATANAS argumentsMinCount 1', filter)
      return 1;
    }
    console.warn('ATANAS argumentsMinCount 0', filter)
    return 0;
  }

  public argumentsMaxCount(filter: FilterDescriptor | FilterOperatorType): number | null {
    if (this.expectsTwoArguments(filter)) {
      return 2;
    }
    if (this.expectsSingleArgument(filter)) {
      return 1;
    }
    if (this.expectsNoArguments(filter)) {
      return 0;
    }
    return null;
  }

  public isFilterType(filter: FilterDescriptor | FilterOperatorType): boolean {
    return !filter.hasOwnProperty('operatorType');
  }
}
