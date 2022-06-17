import {Injectable} from '@angular/core';
import {
  FilterDefinition,
  FilterType,
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

  readonly SINGLE_ARGUMENT_FILTER_TYPES: FilterType[] = [
    'EQ',
    'NOT_EQ',
    'LT',
    'LT_EQ',
    'GT',
    'GT_EQ',
    'CONTAINS',
  ];

  readonly TWO_ARGUMENT_FILTER_TYPES: FilterType[] = [
    'RANGE',
  ];

  readonly DYNAMIC_ARGUMENT_FILTER_TYPES: FilterType[] = [
    'SEQUENCE',
  ];

  readonly NO_ARGUMENT_FILTER_TYPES: FilterType[] = [
    'IS_NULL',
    'IS_NOT_NULL',
    'IS_EMPTY',
    'IS_NOT_EMPTY'
  ];

  constructor() { }

  public getInputControlType(prop: PropertyType): string {
    const result = this.PROPERTY_TYPE_TO_INPUT_TYPE[prop];
    return result ?? 'unknown';
  }

  public expectsDynamicArguments(filter: FilterDefinition | FilterType): boolean {
    const isFilterType = this.isFilterType(filter);
    if (isFilterType) {
      return this.DYNAMIC_ARGUMENT_FILTER_TYPES.includes(filter as FilterType);
    }
    return this.expectsDynamicArguments((filter as FilterDefinition).filterType);
  }

  public expectsTwoArguments(filter: FilterDefinition | FilterType): boolean {
    const isFilterType = this.isFilterType(filter);
    if (isFilterType) {
      return this.TWO_ARGUMENT_FILTER_TYPES.includes(filter as FilterType);
    }
    return this.expectsTwoArguments((filter as FilterDefinition).filterType);
  }

  public expectsSingleArgument(filter: FilterDefinition | FilterType): boolean {
    const isFilterType = this.isFilterType(filter);
    if (isFilterType) {
      return this.SINGLE_ARGUMENT_FILTER_TYPES.includes(filter as FilterType);
    }
    return this.expectsSingleArgument((filter as FilterDefinition).filterType);
  }

  public expectsNoArguments(filter: FilterDefinition | FilterType): boolean {
    const isFilterType = this.isFilterType(filter);
    if (isFilterType) {
      return this.NO_ARGUMENT_FILTER_TYPES.includes(filter as FilterType);
    }
    return this.expectsNoArguments((filter as FilterDefinition).filterType);
  }

  public argumentsMinCount(filter: FilterDefinition | FilterType): number {
    if (this.expectsTwoArguments(filter)) {
      return 2;
    }
    if (this.expectsSingleArgument(filter)) {
      return 1;
    }
    return 0;
  }

  public argumentsMaxCount(filter: FilterDefinition | FilterType): number | null {
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

  public isFilterType(filter: FilterDefinition | FilterType): boolean {
    return !filter.hasOwnProperty('filterType');
  }
}
