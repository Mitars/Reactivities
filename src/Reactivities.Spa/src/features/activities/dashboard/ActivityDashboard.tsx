import React, { useContext, useEffect, useState } from 'react';
import { Grid, Loader } from 'semantic-ui-react';
import ActivityList from './ActivityList';
import { observer } from 'mobx-react-lite';
import { RootStoreContext } from '../../../app/stores/rootStore';
import InfiniteScroll from 'react-infinite-scroller';
import ActivityFilters from './ActivityFilters';
import ActivityListItemPlaceholder from './ActivityListItemPlaceholder';

const ActivityDashboard = () => {
  const rootStore = useContext(RootStoreContext);
  const { loadActivities, setPage, page, totalPages } = rootStore.activityStore;
  const [loadingNext, setLoadingNext] = useState(false);
  const [firstLoad, setFirstLoad] = useState(true);

  const handleGetNext = () => {
    setLoadingNext(true);
    setPage(page + 1);
    loadActivities().then(() => setLoadingNext(false));
  };

  useEffect(() => {
    setFirstLoad(false);
  }, []);

  return (
    <Grid>
      <Grid.Column width={10}>
        <>
          <InfiniteScroll
            pageStart={0}
            loadMore={handleGetNext}
            hasMore={firstLoad || (!loadingNext && page + 1 < totalPages)}
            initialLoad={true}
            useWindow={true}
          >
            <ActivityList />
          </InfiniteScroll>
          {loadingNext && (
            <>
              <ActivityListItemPlaceholder />
              <ActivityListItemPlaceholder />
              <br />
            </>
          )}
        </>
      </Grid.Column>
      <Grid.Column width={6}>
        <ActivityFilters />
      </Grid.Column>
      <Grid.Column width={10}>
        <Loader active={loadingNext} />
      </Grid.Column>
    </Grid>
  );
};

export default observer(ActivityDashboard);
