import {Moment} from 'moment';

export class AppointmentsListModel {
  total: number;
  appointments: Array<AppointmentSimpleModel> = [];
}

export class AppointmentSimpleModel {
  id: number;
  startAt: Moment;
  expireAt: Moment | null;
  title: string;
  description: string;
  info: string;
}
