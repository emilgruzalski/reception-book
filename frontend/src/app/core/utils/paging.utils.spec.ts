import { of } from 'rxjs';

import { PagedResult } from '../models/paged-result.model';
import { fetchAllPages } from './paging.utils';

describe('fetchAllPages', () => {
  const page = (currentPage: number, totalPages: number, items: string[]): PagedResult<string> =>
    ({ items, pagination: { currentPage, totalPages, pageSize: 2, totalCount: 0 } });

  it('should join the items of all pages in order', () => {
    const pages = [page(1, 3, ['a', 'b']), page(2, 3, ['c', 'd']), page(3, 3, ['e'])];
    let result: string[] = [];

    fetchAllPages(number => of(pages[number - 1])).subscribe(items => result = items);

    expect(result).toEqual(['a', 'b', 'c', 'd', 'e']);
  });

  it('should ask only for the first page when there is nothing more', () => {
    const loadPage = jasmine.createSpy().and.returnValue(of(page(1, 0, [])));
    let result: string[] | undefined;

    fetchAllPages<string>(loadPage).subscribe(items => result = items);

    expect(result).toEqual([]);
    expect(loadPage).toHaveBeenCalledOnceWith(1);
  });
});
