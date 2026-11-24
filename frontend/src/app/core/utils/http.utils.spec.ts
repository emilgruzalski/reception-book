import { HttpHeaders, HttpResponse } from '@angular/common/http';

import { toHttpParams, toPagedResult } from './http.utils';

describe('http utils', () => {
  it('should skip empty query values', () => {
    const params = toHttpParams({ pageNumber: 1, searchTerm: '', status: null, type: undefined, orderBy: 'number' });

    expect(params.keys()).toEqual(['pageNumber', 'orderBy']);
  });

  it('should read paging metadata from the X-Pagination header', () => {
    const response = new HttpResponse({
      body: ['a', 'b'],
      headers: new HttpHeaders({
        'X-Pagination': JSON.stringify({ CurrentPage: 2, TotalPages: 3, PageSize: 10, TotalCount: 25 })
      })
    });

    expect(toPagedResult(response)).toEqual({
      items: ['a', 'b'],
      pagination: { currentPage: 2, totalPages: 3, pageSize: 10, totalCount: 25 }
    });
  });
});
