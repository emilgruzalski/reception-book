import { NgModule } from '@angular/core';

import { SharedModule } from 'src/app/shared/shared.module';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard.component';
import { StayChartComponent } from './stay-chart/stay-chart.component';

@NgModule({
  declarations: [
    DashboardComponent,
    StayChartComponent
  ],
  imports: [
    SharedModule,
    DashboardRoutingModule
  ]
})
export class DashboardModule { }
