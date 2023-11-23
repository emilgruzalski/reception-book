import { Injectable } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';

@Injectable()
export class PolishPaginatorIntl extends MatPaginatorIntl {
  override itemsPerPageLabel = 'Wierszy na stronie:';
  override nextPageLabel = 'Następna strona';
  override previousPageLabel = 'Poprzednia strona';
  override firstPageLabel = 'Pierwsza strona';
  override lastPageLabel = 'Ostatnia strona';

  override getRangeLabel = (page: number, pageSize: number, length: number): string => {
    if (length === 0) {
      return `0 z ${length}`;
    }

    const start = page * pageSize + 1;
    const end = Math.min((page + 1) * pageSize, length);

    return `${start}–${end} z ${length}`;
  };
}
