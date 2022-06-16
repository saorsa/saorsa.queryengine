
export type BinaryOperator = 'And' | 'Or';
export type FilterType =
  'IS_NULL' |
  'IS_NOT_NULL' |
  'EQ' |
  'NOT_EQ' |
  'LT' |
  'LT_EQ' |
  'GT' |
  'GT_EQ' |
  'RANGE' |
  'SEQUENCE' |
  'CONTAINS' |
  'IS_EMPTY' |
  'IS_NOT_EMPTY';

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
  type: string;
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
