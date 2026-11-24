/** Delay before a search request is sent while the user is typing. */
export const SEARCH_DEBOUNCE_MS = 400;

/** Minimum number of characters before the autocomplete starts searching. */
export const MIN_SEARCH_LENGTH = 2;

/** Maximum number of guests in a single reservation (must match the API validation). */
export const MAX_GUESTS_PER_RESERVATION = 10;

/** Password rules enforced by ASP.NET Core Identity on the API. */
export const PASSWORD_MIN_LENGTH = 12;
export const PASSWORD_PATTERN = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/;
