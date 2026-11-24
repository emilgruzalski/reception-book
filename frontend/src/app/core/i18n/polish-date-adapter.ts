import { Injectable } from '@angular/core';
import { NativeDateAdapter } from '@angular/material/core';

const POLISH_DATE_PATTERN = /^(\d{1,2})\.(\d{1,2})\.(\d{4})$/;

/**
 * Reads dates typed as dd.MM.yyyy.
 * NativeDateAdapter uses Date.parse(), which rejects 27.11.2026 and reads 05.12.2026 as May 12.
 */
@Injectable()
export class PolishDateAdapter extends NativeDateAdapter {
  override parse(value: unknown): Date | null {
    if (typeof value !== 'string') {
      return super.parse(value);
    }

    const text = value.trim();
    if (!text) {
      return null;
    }

    const match = POLISH_DATE_PATTERN.exec(text);
    if (!match) {
      return this.invalid();
    }

    const [day, month, year] = match.slice(1).map(Number);
    const date = new Date(year, month - 1, day);
    // Date moves days that do not exist to the next month, e.g. 31.02 to 03.03.
    const exists = date.getFullYear() === year && date.getMonth() === month - 1 && date.getDate() === day;

    return exists ? date : this.invalid();
  }
}
