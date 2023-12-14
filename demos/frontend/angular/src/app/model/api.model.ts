import { QueryableTypeDescriptor } from "./query-engine.model";

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

export interface ApiQueryableTypeSingleResult extends ApiResult<QueryableTypeDescriptor> {}

export interface ApiQueryableTypeListResult extends ApiResult<QueryableTypeDescriptor[]> {}

export interface EntityBase {
  testCaseId?: string;
  testSubCaseId?: string;
  createdAtUtc?: string;
  createdBy?: string;
}

export interface Category extends EntityBase {
  id: number;
  name: string;
  parentCategoryId?: number;
  parentCategory?: Category;
}

export interface Group extends EntityBase {
  id: string;
  categoryId?: number;
  category?: Category;
}

export type UserLogonType = 'ActiveDirectory' | 'Oidc' | 'Saml';

export interface User extends EntityBase {
  id: string;
  username: string;
  password?: string;
  gender?: string;
  externalId?: number;
  latestLogonType?: UserLogonType;
  categoryId?: number;
  category?: Category;
}
