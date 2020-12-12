import React from 'react';
import { Link } from 'react-router-dom';
import { Button, Icon, Item, Label, Segment } from 'semantic-ui-react';
import { Activity } from '../../../app/models/activity';
import { format } from 'date-fns';
import { ActivityListItemAttendees } from './ActivityListItemAttendees';

export const ActivityListItem = ({ activity }: { activity: Activity }) => {
  const host = activity.attendees.filter((x) => x.isHost)[0];
  return (
    <Segment.Group>
      <Segment>
        <Item.Group>
          <Item key={activity.id}>
            <Item.Image
              size='tiny'
              circular
              src={host.image || '/assets/user.png'}
              style={{marginBottom: 3}}
            />
            <Item.Content>
              <Item.Header as={Link} to={`/activities/${activity.id}`}>
                {activity.title}
              </Item.Header>
              <Item.Description>Hosted by<Link to={`/profile/${host.userName}`}> {host.displayName}</Link></Item.Description>
              {activity.isHost && (
                <Item.Description>
                  <Label
                    basic
                    color='orange'
                    content='You are hosting this activity'
                  ></Label>
                </Item.Description>
              )}
              {activity.isGoing && !activity.isHost && (
                <Item.Description>
                  <Label
                    basic
                    color='green'
                    content='You are going to this activity'
                  ></Label>
                </Item.Description>
              )}
            </Item.Content>
          </Item>
        </Item.Group>
      </Segment>
      <Segment>
        <Icon name='clock' />
        {format(activity.date, 'h:mm a')}
        <Icon name='marker' />
        {activity.venue}, {activity.city}
      </Segment>
      <Segment secondary>
        <ActivityListItemAttendees
          attendees={activity.attendees}
        ></ActivityListItemAttendees>
      </Segment>
      <Segment clearing>
        <span>{activity.description}</span>
        <Button
          as={Link}
          to={`/activities/${activity.id}`}
          floated='right'
          content='View'
          color='blue'
        />
      </Segment>
    </Segment.Group>
  );
};
