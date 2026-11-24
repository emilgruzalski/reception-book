import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';

const SERVER_ERROR_STATUS = 500;

/** Builds a user-friendly message from an API error response. */
export function getErrorMessage(error: HttpErrorResponse): string {
  if (error.status === 0) {
    return 'Brak połączenia z serwerem. Sprawdź połączenie z siecią.';
  }

  if (error.status >= SERVER_ERROR_STATUS) {
    return 'Wystąpił błąd serwera. Spróbuj ponownie później.';
  }

  switch (error.status) {
    case HttpStatusCode.Forbidden:
      return 'Nie masz uprawnień do wykonania tej operacji.';
    case HttpStatusCode.TooManyRequests:
      return 'Zbyt wiele żądań. Spróbuj ponownie za chwilę.';
    case HttpStatusCode.UnprocessableEntity:
      return `Formularz zawiera niepoprawne dane. ${getValidationErrors(error.error)}`.trim();
    default:
      return getApiMessage(error.error) ?? 'Nie udało się wykonać operacji.';
  }
}

/** The API returns either { StatusCode, Message } or a dictionary of validation errors. */
function getApiMessage(body: unknown): string | null {
  if (!body) {
    return null;
  }

  if (typeof body === 'string') {
    return body;
  }

  if (typeof body === 'object' && 'Message' in body && typeof body.Message === 'string') {
    return body.Message;
  }

  return getValidationErrors(body) || null;
}

function getValidationErrors(body: unknown): string {
  if (!body || typeof body !== 'object') {
    return '';
  }

  return Object.values(body)
    .flat()
    .filter((value): value is string => typeof value === 'string')
    .join(' ');
}
