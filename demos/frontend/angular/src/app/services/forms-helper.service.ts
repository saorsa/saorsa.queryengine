import { Injectable } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormGroup,
  ValidationErrors
} from "@angular/forms";

@Injectable({
  providedIn: 'root'
})
export class FormsHelperService {

  constructor() { }

  getErrorsInDepth(c: AbstractControl, errorKeyPrefix?: any): ValidationErrors | null {
    if (c.errors) return c.errors;
    let aggregateResult: ValidationErrors = {};
    if (c instanceof FormArray) {
      const fg = c as FormArray;
      fg.controls.forEach((childControl, index) => {
        const ve = this.getErrorsInDepth(childControl, `[${index}]`);
        if (ve) {
          Object.getOwnPropertyNames(ve).forEach(key => {
            const errKey = errorKeyPrefix ? `${errorKeyPrefix}.[${index}].${key}` : `[${index}].${key}`;
            aggregateResult[errKey] = ve[key];
          });
        }
      });
    }
    else if (c instanceof FormGroup) {
      const fg = c as FormGroup;
      Object.getOwnPropertyNames(fg.controls)
        .forEach(key => {
          const childControl = fg.get(key)
          if (childControl != null) {
            const ve = this.getErrorsInDepth(childControl, key);
            if (ve) {
              Object.getOwnPropertyNames(ve).forEach(nestedErrorKey => {
                const errKey = errorKeyPrefix ? `${errorKeyPrefix}.${nestedErrorKey}` : nestedErrorKey;
                aggregateResult[errKey] = ve[nestedErrorKey];
              });
            }
          }
        });
    }
    const hasErrors = Object.getOwnPropertyNames(aggregateResult).length > 0;
    return hasErrors ? aggregateResult : null;
  }
}
