
export type BinaryOperator = 'And' | 'Or';
export type FilterType =
  'IS_NULL' | 'IS_NOT_NULL'
  | 'EQ' | 'NOT_EQ'
  | 'LT' | 'LT_EQ'
  | 'GT' | 'GT_EQ'
  | 'RANGE' | 'SEQUENCE' | 'CONTAINS'
  | 'IS_EMPTY' | 'IS_NOT_EMPTY';

export type FilterTypeBooleanSwitch = {
  [key in FilterType]: boolean;
};

export type FilterTypeStringMap = {
  [key in FilterType]: string | null;
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
  [key in PropertyType]: string | null;
};

export interface FilterDefinition{
  filterType: FilterType;
  arg1?: string;
  arg1Required?: boolean;
  arg2?: string;
  arg2Required?: boolean;
}

export interface TypeDefinition {
  name: string;
  typeName: string;
  nullable: boolean;
  type: PropertyType;
  enumValues?: string[];
  properties?: TypeDefinition[];
  allowedFilters?: FilterDefinition[];
  arrayElement?: TypeDefinition;
}

export interface PropertyFilter {
  name: string;
  filterType: FilterType;
  arguments: [];
}

export interface PropertyFilterBlock {
  first: PropertyFilter;
  condition?: BinaryOperator;
  others?: PropertyFilterBlock[];
}
