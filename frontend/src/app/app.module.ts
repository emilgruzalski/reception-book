import { DEFAULT_CURRENCY_CODE, LOCALE_ID, NgModule } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localePl from '@angular/common/locales/pl';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LayoutModule } from './layout/layout.module';
import { PolishPaginatorIntl } from './core/i18n/polish-paginator-intl';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { ErrorInterceptor } from './core/interceptors/error.interceptor';

registerLocaleData(localePl);

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    LayoutModule,
    MatSnackBarModule
  ],
  providers: [
    { provide: LOCALE_ID, useValue: 'pl' },
    { provide: MAT_DATE_LOCALE, useValue: 'pl-PL' },
    { provide: DEFAULT_CURRENCY_CODE, useValue: 'PLN' },
    { provide: MatPaginatorIntl, useClass: PolishPaginatorIntl },
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
