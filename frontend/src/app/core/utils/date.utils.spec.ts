import { fromApiDate, toApiDate } from './date.utils';

describe('date utils', () => {
  it('should keep the selected day regardless of the time zone', () => {
    expect(toApiDate(new Date(2023, 10, 20))).toBe('2023-11-20');
  });

  it('should parse dates from the API as local dates', () => {
    const date = fromApiDate('2023-11-20T00:00:00');

    expect(date.getFullYear()).toBe(2023);
    expect(date.getMonth()).toBe(10);
    expect(date.getDate()).toBe(20);
  });
});
