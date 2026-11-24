import { EMPTY, expand, Observable, reduce } from 'rxjs';

import { PagedResult } from '../models/paged-result.model';

/** Largest page the API returns. */
export const MAX_PAGE_SIZE = 50;

/** Loads all pages one after another and returns their items as a single list. */
export function fetchAllPages<T>(loadPage: (pageNumber: number) => Observable<PagedResult<T>>): Observable<T[]> {
  return loadPage(1).pipe(
    expand(({ pagination }) =>
      pagination.currentPage < pagination.totalPages ? loadPage(pagination.currentPage + 1) : EMPTY),
    reduce((items, page) => [...items, ...page.items], [] as T[])
  );
}
