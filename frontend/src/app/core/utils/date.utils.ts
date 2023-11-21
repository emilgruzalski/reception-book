import { formatDate } from '@angular/common';

const API_DATE_FORMAT = 'yyyy-MM-dd';
const API_DATE_LOCALE = 'en-US';
const API_DATE_LENGTH = API_DATE_FORMAT.length;

/**
 * Formats a date as yyyy-MM-dd using local time.
 * Date.toISOString() converts to UTC, which moves local midnight to the previous day in Poland.
 */
export function toApiDate(date: Date): string {
  return formatDate(date, API_DATE_FORMAT, API_DATE_LOCALE);
}

/** Parses a date returned by the API (yyyy-MM-ddT00:00:00) as a local date. */
export function fromApiDate(value: string): Date {
  const [year, month, day] = value.substring(0, API_DATE_LENGTH).split('-').map(Number);
  return new Date(year, month - 1, day);
}
