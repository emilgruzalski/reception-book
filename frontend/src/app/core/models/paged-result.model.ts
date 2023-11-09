export const DEFAULT_PAGE_SIZE = 10;

export const PAGE_SIZE_OPTIONS = [10, 25, 50];

export interface PageQuery {
  pageNumber: number;
  pageSize: number;
  orderBy?: string;
}

export interface Pagination {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
}

export interface PagedResult<T> {
  items: T[];
  pagination: Pagination;
}
