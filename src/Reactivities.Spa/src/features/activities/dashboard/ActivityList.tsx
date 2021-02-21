import { observer } from 'mobx-react-lite';
import React, { Fragment, useContext } from 'react';
import { Item, Label } from 'semantic-ui-react';
import { RootStoreContext } from '../../../app/stores/rootStore';
import { ActivityListItem } from './ActivityListItem';
import { format } from 'date-fns';

const ActivityList = () => {
  const rootStoreContext = useContext(RootStoreContext);
  const { activitiesByDate } = rootStoreContext.activityStore;

  return (
    <Fragment>
      {activitiesByDate.map(([group, activities]) => (
        <Fragment key={group}>
          <Label size="large" color="blue">
            {format(new Date(group), 'eeee do MMMM')}
          </Label>
          <Item.Group divided>
            {activities.map((activity) => (
              <ActivityListItem key={activity.id} activity={activity} />
            ))}
          </Item.Group>
        </Fragment>
      ))}
    </Fragment>
  );
};

export default observer(ActivityList);
