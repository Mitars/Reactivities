export interface Activity {
  id: string;
  title: string;
  description: string;
  category: string;
  date: Date;
  city: string;
  venue: string;
}

export class ActivityFormValues implements Partial<Activity> {
  id?: string = undefined;
  title: string = '';
  category: string = '';
  description: string = '';
  date?: Date = undefined;
  time?: Date = undefined;
  city: string = '';
  venue: string = '';

  constructor(init?: Activity) {
    Object.assign(this, init);
    if (init && init.date) {
      this.time = init.date;
    }
  }
}
