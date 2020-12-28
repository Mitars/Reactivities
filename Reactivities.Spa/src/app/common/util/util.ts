import { Activity, Attendee } from '../../models/activity';
import { User } from '../../models/user';

export const combineDateAndTime = (date: Date, time: Date) => {
  const dateString = date.toISOString().split('T')[0];
  const timeString = time.toISOString().split('T')[1];
  return new Date(dateString + ' ' + timeString);
};

export const setActivityProps = (activity: Activity, user: User) => {
  activity.date = new Date(activity.date);
  activity.isGoing = activity.attendees.some(
    (a) => a.userName === user.userName
  );
  activity.isHost = activity.attendees.some(
    (a) => a.userName === user.userName && a.isHost
  );
  return activity;
};

export const createAttendee = (user: User): Attendee => {
  return {
    displayName: user.displayName,
    isHost: false,
    userName: user.userName,
    image: user.image!,
  };
};
