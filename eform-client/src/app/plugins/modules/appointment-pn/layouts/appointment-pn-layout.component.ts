import {AfterViewInit, Component, OnInit} from '@angular/core';
import {LocaleService} from '../../../../common/services/auth';
import {TranslateService} from '@ngx-translate/core';
import {SharedPnService} from '../../shared/services';
declare var require: any;

@Component({
  selector: 'app-appointment-pn-layout',
  template: '<router-outlet></router-outlet>'
})
export class AppointmentPnLayoutComponent implements  AfterViewInit, OnInit {
  constructor(private localeService: LocaleService,
              private translateService: TranslateService,
              private sharedPnService: SharedPnService) {
  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
  }
}
