import { HttpParams, HttpResponse } from '@angular/common/http';

import { PagedResult } from '../models/paged-result.model';

const PAGINATION_HEADER = 'X-Pagination';

/** Converts a query object to HTTP params, skipping empty values. */
export function toHttpParams(query: object): HttpParams {
  return Object.entries(query).reduce(
    (params, [key, value]) =>
      value === null || value === undefined || value === '' ? params : params.set(key, String(value)),
    new HttpParams()
  );
}

/** Reads the items from the body and the paging metadata from the X-Pagination header. */
export function toPagedResult<T>(response: HttpResponse<T[]>): PagedResult<T> {
  const header = response.headers.get(PAGINATION_HEADER);
  const metaData = header ? JSON.parse(header) : {};

  return {
    items: response.body ?? [],
    pagination: {
      currentPage: metaData.CurrentPage ?? 1,
      totalPages: metaData.TotalPages ?? 0,
      pageSize: metaData.PageSize ?? 0,
      totalCount: metaData.TotalCount ?? 0
    }
  };
}
