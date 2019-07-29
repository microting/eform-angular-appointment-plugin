import {Component, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';
import {AppointmentModel} from '../../../models';
import {AppointmentPnCalendarService} from '../../../services';
import * as moment from 'moment';

@Component({
  selector: 'app-appointment-edit',
  templateUrl: './appointment-edit.component.html',
  styleUrls: ['./appointment-edit.component.scss']
})
export class AppointmentEditComponent implements OnInit {
  @ViewChild('frame') frame;
  @Output() appointmentSaved: EventEmitter<void> = new EventEmitter<void>();
  spinnerStatus = false;
  selectedModel: AppointmentModel = new AppointmentModel();

  constructor(private appointmentPnCalendarService: AppointmentPnCalendarService) {
  }

  ngOnInit() {
  }

  show(id?: number) {
    if (id) {
      this.spinnerStatus = true;
      this.appointmentPnCalendarService.getAppointment(id).subscribe((data) => {
        if (data && data.success) {
          this.selectedModel = data.model;
          this.selectedModel.startAt = moment(this.selectedModel.startAt);
          this.selectedModel.expireAt = moment(this.selectedModel.expireAt);
        }
        this.frame.show();
        this.spinnerStatus = false;
      });
    } else {
      this.frame.show();
    }
  }

  saveAppointment() {
    this.spinnerStatus = true;
    this.selectedModel.startAt.utcOffset(0, true);

    if (this.selectedModel.expireAt) {
      this.selectedModel.expireAt.utcOffset(0, true);
    }

    if (this.selectedModel.id) {
      this.appointmentPnCalendarService.updateAppointment(this.selectedModel)
        .subscribe((data) => {
          if (data && data.success) {
            this.appointmentSaved.emit();
            this.selectedModel = new AppointmentModel();
            this.frame.hide();
          } this.spinnerStatus = false;
        });
    } else {
      this.appointmentPnCalendarService.createAppointment(this.selectedModel)
        .subscribe((data) => {
          if (data && data.success) {
            this.appointmentSaved.emit();
            this.selectedModel = new AppointmentModel();
            this.frame.hide();
          } this.spinnerStatus = false;
        });
    }
  }
}
