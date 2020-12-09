import React from 'react';
import { List, Image, Popup } from 'semantic-ui-react';
import { Attendee } from '../../../app/models/activity';

export const ActivityListItemAttendees = ({
  attendees,
}: {
  attendees: Attendee[];
}) => {
  return (
    <List horizontal>
      {attendees.map((attendee) => (
        <List.Item key={attendee.userName}>
          <Popup
            header={attendee.displayName}
            trigger={
              <Image
                size='mini'
                circular
                src={attendee.image || '/assets/user.png'}
              />
            }
          ></Popup>
        </List.Item>
      ))}
    </List>
  );
};
