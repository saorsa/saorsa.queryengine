/**
 * Logical operators are used to perform logical operation such as and, or. Logical operators operates on boolean
 * expressions and returns boolean values (true / false).
 */
export type LogicalOperator = 'And' | 'Or'

/**
 * Enumeration with the filter operators types supported by Query Engine.
 */
export type FilterOperatorType =
  'IsNull' | 'IsNotNull'
  | 'EqualTo' | 'NotEqualTo'
  | 'LessThan' | 'LessThanOrEqual'
  | 'GreaterThan' | 'GreaterThanOrEqual'
  | 'ValueInRange' | 'ValueInSequence' | 'StringContains'
  | 'CollectionIsEmpty' | 'CollectionIsNotEmpty'

export type FilterTypeBooleanSwitch = {
  [key in FilterOperatorType]: boolean
};

export type FilterTypeStringMap = {
  [key in FilterOperatorType]: string | null
};

export type PropertyType =
  'char' | 'boolean' | 'byte' | 'sbyte'
  | 'int16' | 'integer' | 'int64'
  | 'uint16' | 'uint32' | 'uint64'
  | 'float' | 'double' | 'decimal'
  | 'string' | 'uuid'
  | 'date' | 'dateTime' | 'dateTimeOffset'
  | 'time' | 'timeSpan'
  | 'enum'
  | 'array'
  | 'object';

export type PropertyTypeStringMap = {
  [key in PropertyType]: string | null
}

/**
 * Object that carries description and meta-data about a filter expression - its structure and operands.
 */
export interface FilterDescriptor {
  operatorType: FilterOperatorType
  arg1?: string
  arg1Required?: boolean
  arg2?: string
  arg2Required?: boolean
  description?: string
}

/**
 * Object that carries meta description about a type that can be used in dynamic queries used by
 * the Query Engine runtime.
 */
export interface QueryableTypeDescriptor {
  name: string
  typeName: string
  nullable: boolean
  type: PropertyType
  enumValues?: string[]
  properties?: QueryableTypeDescriptor[]
  allowedFilters?: FilterDescriptor[]
  arrayElement?: QueryableTypeDescriptor
}

export interface PropertyFilter {
  name: string
  filterType: FilterOperatorType
  arguments: any[]
}

export interface PropertyFilterBlock {
  first: PropertyFilter
  condition?: LogicalOperator
  others?: PropertyFilterBlock[]
}
