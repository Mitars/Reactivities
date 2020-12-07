import React, { useContext, useEffect } from 'react';
import { Grid } from 'semantic-ui-react';
import { LoadingComponent } from './../../../app/layout/LoadingComponent';
import ActivityList from './ActivityList';
import { observer } from 'mobx-react-lite';
import { RootStoreContext } from '../../../app/stores/rootStore';

const ActivityDashboard = () => {
  const rootStore = useContext(RootStoreContext);
  const { loadActivities, loadingInitial } = rootStore.activityStore;

  useEffect(() => {
    loadActivities();
  }, [loadActivities]);

  if (loadingInitial)
    return <LoadingComponent content="Loading Activities..." />;

  return (
    <Grid>
      <Grid.Column width={10}>
        <ActivityList />
      </Grid.Column>
      <Grid.Column width={6}>
        <h2>Activity Filters</h2>
      </Grid.Column>
    </Grid>
  );
};

export default observer(ActivityDashboard);
