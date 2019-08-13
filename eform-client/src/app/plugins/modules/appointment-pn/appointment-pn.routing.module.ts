import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AppointmentPnLayoutComponent} from './layouts';
import {AdminGuard} from '../../../common/guards';
import {AppointmentCalendarComponent, AppointmentSettingsComponent} from './components';

export const routes: Routes = [
  {
    path: '',
    component: AppointmentPnLayoutComponent,
    children: [
      {
        path: 'settings',
        canActivate: [AdminGuard],
        component: AppointmentSettingsComponent
      },
      {
        path: 'calendar',
        canActivate: [AdminGuard],
        component: AppointmentCalendarComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AppointmentPnRoutingModule {
}
