import {Moment} from 'moment';

export class AppointmentModel {
  id: number;
  startAt: Moment;
  expireAt: Moment | null;
  title: string;
  description: string;
  info: string;
}
