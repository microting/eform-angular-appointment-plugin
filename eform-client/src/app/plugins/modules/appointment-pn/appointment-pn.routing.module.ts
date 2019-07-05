import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AppointmentPnLayoutComponent} from './layouts';
import {AdminGuard} from '../../../common/guards';
import {AppointmentSettingsComponent} from './components';

export const routes: Routes = [
  {
    path: '',
    component: AppointmentPnLayoutComponent,
    children: [
      {
        path: 'settings',
        canActivate: [AdminGuard],
        component: AppointmentSettingsComponent
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
