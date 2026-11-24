import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/** Form group validator that checks whether two fields have the same value. */
export function matchFieldsValidator(field: string, confirmationField: string): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const value = group.get(field)?.value;
    const confirmation = group.get(confirmationField)?.value;

    return value && confirmation && value !== confirmation ? { fieldsMismatch: true } : null;
  };
}
