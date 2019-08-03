import {Moment} from 'moment';

export enum RepeatType {
  Day = 1,
  Week,
  Month
}

export class AppointmentModel {
  id: number;
  startAt: Moment;
  expireAt: Moment | null;
  title: string;
  description: string;
  info: string;
  colorHex: string;
  repeatEvery: number;
  repeatType: number;
  repeatUntil: Moment;
  nextId: number;
}
