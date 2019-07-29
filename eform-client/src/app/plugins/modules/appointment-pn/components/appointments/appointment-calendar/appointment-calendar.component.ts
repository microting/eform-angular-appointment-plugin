import {ChangeDetectionStrategy, Component, OnInit, ViewChild} from '@angular/core';
import {CalendarEventAction, CalendarEventTimesChangedEvent, CalendarView, CalendarEvent} from 'angular-calendar';
import {isSameDay, isSameMonth} from 'date-fns';
import {Subject} from 'rxjs';
import {AppointmentModel, AppointmentsListModel} from '../../../models';
import {AppointmentPnCalendarService} from '../../../services';
import * as moment from 'moment';

@Component({
  selector: 'app-appointment-calendar',
  templateUrl: './appointment-calendar.component.html',
  styleUrls: ['./appointment-calendar.component.scss']
})
export class AppointmentCalendarComponent implements OnInit {
  @ViewChild('editAppointmentModal') editAppointmentModal;
  @ViewChild('viewAppointmentModal') viewAppointmentModal;
  @ViewChild('deleteAppointmentModal') deleteAppointmentModal;
  spinnerStatus = false;
  view: CalendarView = CalendarView.Month;
  CalendarView = CalendarView;
  viewDate: Date = new Date();
  actions: CalendarEventAction[] = [
    {
      label: '<span class="mx-1 text-success">Edit</span>',
      onClick: ({ event }: { event: CalendarEvent }): void => {
        this.editEvent(event);
      }
    },
    {
      label: '<span class="mx-1 text-danger">Delete</span>',
      onClick: ({ event }: { event: CalendarEvent }): void => {
        this.deleteEvent(event);
      }
    }
  ];
  refresh: Subject<any> = new Subject();
  events: CalendarEvent[] = [];
  activeDayIsOpen = true;
  appointmentsListModel: AppointmentsListModel;

  constructor(
    private appointmentPnCalendarService: AppointmentPnCalendarService
  ) {}

  ngOnInit(): void {
    this.getAppointmentsList();
  }

  getAppointmentsList() {
    this.spinnerStatus = true;
    this.events = [];
    this.appointmentPnCalendarService.getAppointmentsList().subscribe((data) => {
      if (data && data.success) {
        this.appointmentsListModel = data.model;
        for (const a of this.appointmentsListModel.appointments) {
          a.startAt = moment(a.startAt);
          a.expireAt = moment(a.expireAt);

          this.events = [
            ...this.events,
            {
              id: a.id,
              start: a.startAt.toDate(),
              end: a.expireAt.toDate(),
              title: a.title,
              color: {
                primary: '#1e90ff',
                secondary: '#D1E8FF'
              },
              actions: this.actions,
              allDay: true,
              resizable: {
                beforeStart: true,
                afterEnd: true
              },
              draggable: false
            }
          ];
        }
      }
      this.spinnerStatus = false;
    });
  }

  dayClicked({ date, events }: { date: Date; events: CalendarEvent[] }): void {
    if (isSameMonth(date, this.viewDate)) {
      this.activeDayIsOpen = !((isSameDay(this.viewDate, date) && this.activeDayIsOpen === true) || events.length === 0);
      this.viewDate = date;
    }
  }

  eventTimesChanged({event, newStart, newEnd}: CalendarEventTimesChangedEvent): void {
    // this.events = this.events.map(iEvent => {
    //   if (iEvent === event) {
    //     return {
    //       ...event,
    //       start: newStart,
    //       end: newEnd
    //     };
    //   }
    //   return iEvent;
    // });
  }

  showEditAppointmentModal(appointmentModel?: AppointmentModel): void {
    debugger;
    this.editAppointmentModal.show(appointmentModel);
  }

  viewEvent(eventToView: CalendarEvent) {
    this.appointmentPnCalendarService.getAppointment(eventToView.id).subscribe(data => {
      if (data && data.success) {
        this.viewAppointmentModal.show(data.model);
      }
    });
  }

  editEvent(eventToEdit: CalendarEvent) {
    this.appointmentPnCalendarService.getAppointment(eventToEdit.id).subscribe(data => {
      if (data && data.success) {
        this.showEditAppointmentModal(data.model);
      }
    });
  }

  deleteEvent(eventToDelete: CalendarEvent) {
    this.appointmentPnCalendarService.getAppointment(eventToDelete.id).subscribe(data => {
      if (data && data.success) {
        this.deleteAppointmentModal.show(data.model);
      }
    });
  }

  setView(view: CalendarView) {
    this.view = view;
  }

  closeOpenMonthViewDay() {
    this.activeDayIsOpen = false;
  }
}
