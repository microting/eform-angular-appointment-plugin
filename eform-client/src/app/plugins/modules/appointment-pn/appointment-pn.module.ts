import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { MDBBootstrapModule } from 'port/angular-bootstrap-md';
import { NgSelectModule } from '@ng-select/ng-select';
import { SharedPnModule } from '../shared/shared-pn.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { EformSharedModule } from '../../../common/modules/eform-shared/eform-shared.module';
import {AppointmentPnLayoutComponent} from './layouts';

import { AppointmentPnSettingsService} from './services';

import {
  AppointmentSettingsComponent,
} from './components';
import {AppointmentPnRoutingModule} from './appointment-pn.routing.module';
// import {RouterModule} from '@angular/router';

@NgModule({
  imports: [
    CommonModule,
    SharedPnModule,
    MDBBootstrapModule,
    AppointmentPnRoutingModule,
    TranslateModule,
    FormsModule,
    NgSelectModule,
    EformSharedModule,
    FontAwesomeModule
  ],
  declarations: [AppointmentPnLayoutComponent,
    AppointmentSettingsComponent],
  providers: [AppointmentPnSettingsService]
})

export class AppointmentPnModule { }
