export interface Activity {
  id: string;
  title: string;
  description: string;
  category: string;
  date: Date;
  city: string;
  venue: string;
  attendees: Attendee[];
  comments: Comment[];

  isGoing: boolean;
  isHost: boolean;
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

export interface Attendee {
  userName: string;
  displayName: string;
  image: string;
  isHost: boolean;
  following?: boolean;
}

export interface Comment {
  id: string;
  createdAt: Date;
  body: string;
  username: string;
  displayName: string;
  image: string;
}
