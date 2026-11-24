import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';

import { getErrorMessage } from './error.utils';

describe('getErrorMessage', () => {
  it('should return the message sent by the API', () => {
    const error = new HttpErrorResponse({
      status: HttpStatusCode.BadRequest,
      error: { StatusCode: 400, Message: 'Room 101 is already reserved in the selected period.' }
    });

    expect(getErrorMessage(error)).toBe('Room 101 is already reserved in the selected period.');
  });

  it('should join Identity errors returned as a dictionary', () => {
    const error = new HttpErrorResponse({
      status: HttpStatusCode.BadRequest,
      error: { DuplicateUserName: ['Username \'anowak\' is already taken.'] }
    });

    expect(getErrorMessage(error)).toBe('Username \'anowak\' is already taken.');
  });

  it('should not show server error details', () => {
    const error = new HttpErrorResponse({ status: HttpStatusCode.InternalServerError, error: 'NullReferenceException at ...' });

    expect(getErrorMessage(error)).toBe('Wystąpił błąd serwera. Spróbuj ponownie później.');
  });

  it('should handle a missing connection', () => {
    const error = new HttpErrorResponse({ status: 0 });

    expect(getErrorMessage(error)).toContain('Brak połączenia');
  });
});
