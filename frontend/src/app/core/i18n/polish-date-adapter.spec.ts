import { TestBed } from '@angular/core/testing';
import { MAT_DATE_LOCALE } from '@angular/material/core';

import { PolishDateAdapter } from './polish-date-adapter';

describe('PolishDateAdapter', () => {
  let adapter: PolishDateAdapter;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PolishDateAdapter, { provide: MAT_DATE_LOCALE, useValue: 'pl-PL' }]
    });
    adapter = TestBed.inject(PolishDateAdapter);
  });

  it('should read the day before the month', () => {
    expect(adapter.parse('05.12.2026')).toEqual(new Date(2026, 11, 5));
    expect(adapter.parse('27.11.2026')).toEqual(new Date(2026, 10, 27));
  });

  it('should accept days and months without leading zeros', () => {
    expect(adapter.parse(' 5.1.2027 ')).toEqual(new Date(2027, 0, 5));
  });

  it('should read back the dates it displays', () => {
    const date = new Date(2026, 11, 5);

    expect(adapter.parse(adapter.format(date, { year: 'numeric', month: 'numeric', day: 'numeric' }))).toEqual(date);
  });

  it('should return null when the field is empty', () => {
    expect(adapter.parse('')).toBeNull();
    expect(adapter.parse('  ')).toBeNull();
  });

  it('should reject dates that do not exist or are incomplete', () => {
    for (const value of ['31.02.2027', '00.12.2026', '05.13.2026', '05.12', '12/05/2026', 'abc']) {
      const date = adapter.parse(value);

      expect(date).withContext(value).not.toBeNull();
      expect(adapter.isValid(date!)).withContext(value).toBeFalse();
    }
  });
});
