import {
  action,
  computed,
  makeObservable,
  observable,
  configure,
  runInAction,
} from 'mobx';
import { createContext, SyntheticEvent } from 'react';
import agent from '../api/agent';
import { Activity } from '../models/activity';

configure({ enforceActions: 'always' });

class ActivityStore {
  constructor() {
    makeObservable(this);
  }

  @observable activityRegistry = new Map();
  @observable activity: Activity | null = null;
  @observable loadingInitial = false;
  @observable submitting = false;
  @observable target = '';

  @computed get activitiesByDate() {
    return this.groupActivitiesByDate(
      Array.from(this.activityRegistry.values())
    );
  }

  groupActivitiesByDate(activities: Activity[]) {
    const sortedActivities = activities.sort(
      (a, b) => Date.parse(a.date) - Date.parse(b.date)
    );

    return Object.entries(
      sortedActivities.reduce((activities, activity) => {
        const date = activity.date.split('T')[0];
        activities[date] = activities[date]
          ? [...activities[date], activity]
          : [activity];
        return activities;
      }, {} as { [key: string]: Activity[] })
    );
  }

  @action loadActivities = () => {
    this.loadingInitial = true;
    agent.Activities.list()
      .then((activities) =>
        runInAction(() => {
          activities.forEach((activity) => {
            activity.date = activity.date.split('.')[0];
            this.activityRegistry.set(activity.id, activity);
          });
        })
      )
      .finally(() => runInAction(() => (this.loadingInitial = false)));
  };

  @action createActivity = async (activity: Activity) => {
    this.submitting = true;
    agent.Activities.create(activity)
      .then(() =>
        runInAction(() => {
          this.activityRegistry.set(activity.id, activity);
        })
      )
      .finally(() => runInAction(() => (this.submitting = false)));
  };

  @action editActivity = async (activity: Activity) => {
    this.submitting = true;
    agent.Activities.update(activity)
      .then(() =>
        runInAction(() => {
          this.activityRegistry.set(activity.id, activity);
          this.activity = activity;
        })
      )
      .finally(() => runInAction(() => (this.submitting = false)));
  };

  @action deleteActivity = (
    event: SyntheticEvent<HTMLButtonElement>,
    id: string
  ) => {
    this.submitting = true;
    this.target = event.currentTarget.name;
    agent.Activities.delete(id)
      .then(() =>
        runInAction(() => {
          this.activityRegistry.delete(id);
        })
      )
      .then(() =>
        runInAction(() => {
          if (!this.activityRegistry.has(this.activity?.id)) {
            this.activity = null;
          }
        })
      )
      .finally(() =>
        runInAction(() => {
          this.submitting = false;
          this.target = '';
        })
      );
  };

  @action loadActivity = async (id: string) => {
    let activity = this.getActivity(id);
    if (activity) {
      this.activity = activity;
    } else {
      this.loadingInitial = true;
      try {
        activity = await agent.Activities.details(id);
        runInAction(() => {
          this.activity = activity;
          this.loadingInitial = false;
        });
      } catch (error) {
        runInAction(() => {
          this.loadingInitial = false;
        });
        console.log(error);
      }
    }
  };

  @action clearActivity = () => {
    this.activity = null;
  };

  getActivity = (id: string) => {
    return this.activityRegistry.get(id);
  };
}

export default createContext(new ActivityStore());
