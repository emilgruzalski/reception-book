import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export const PHONE_PATTERN = /^\+?[0-9 ()-]{7,20}$/;
export const DOCUMENT_NUMBER_PATTERN = /^[A-Za-z0-9]{5,30}$/;
export const ROOM_NUMBER_PATTERN = /^[A-Za-z0-9-]{1,10}$/;

/** Form group validator which requires the end date to be after the start date. */
export function dateRangeValidator(startField: string, endField: string): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const start: Date | null = group.get(startField)?.value;
    const end: Date | null = group.get(endField)?.value;

    return start && end && end <= start ? { invalidDateRange: true } : null;
  };
}
