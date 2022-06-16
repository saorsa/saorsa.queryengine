import {TypeDefinition} from "./query-engine.model";

export type ApiResultStatus = 'Ok' | 'Warning' | 'Error' | 'Fatal';

export interface ApiResult<T> {
  timestamp: Date;
  refId: string;
  status?: ApiResultStatus;
  message?: string;
  code?: number;
  context?: any;
  refType?: any;
  result?: T;
}

export interface ApiHealthResult extends ApiResult<any> {}

export interface ApiTypeDefinitionSingleResult extends ApiResult<TypeDefinition> {}

export interface ApiTypeDefinitionsResult extends ApiResult<TypeDefinition[]> {}
