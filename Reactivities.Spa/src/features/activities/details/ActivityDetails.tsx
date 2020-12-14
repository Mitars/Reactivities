import { observer } from 'mobx-react-lite';
import React, { useContext, useEffect, useState } from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { Grid } from 'semantic-ui-react';
import { LoadingComponent } from '../../../app/layout/LoadingComponent';
import ActivityDetailedHeader from './ActivityDetailedHeader';
import { ActivityDetailedInfo } from './ActivityDetailedInfo';
import ActivityDetailedChat from './ActivityDetailedChat';
import ActivityDetailedSidebar from './ActivityDetailedSidebar';
import { RootStoreContext } from '../../../app/stores/rootStore';

const ActivityDetails = ({ match }: RouteComponentProps<{ id: string }>) => {
  const rootStore = useContext(RootStoreContext);
  const { activity, loadActivity, loadingInitial } = rootStore.activityStore;
  const [ initialLoad, setInitialLoad] = useState(true);

  useEffect(() => {
    loadActivity(match.params.id).then(() => setInitialLoad(false));
  }, [loadActivity, match.params.id]);

  if (loadingInitial || !activity)
    return <LoadingComponent content='Loading Activity...' />;

  return (
    <Grid>
      <Grid.Column width={10}>
        <ActivityDetailedHeader activity={activity} />
        <ActivityDetailedInfo activity={activity} />
        {!initialLoad && <ActivityDetailedChat/>}
      </Grid.Column>
      <Grid.Column width={6}>
        <ActivityDetailedSidebar
          attendees={activity.attendees}
        ></ActivityDetailedSidebar>
      </Grid.Column>
    </Grid>
  );
};

export default observer(ActivityDetails);
