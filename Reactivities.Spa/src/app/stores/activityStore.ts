import {
  action,
  computed,
  makeObservable,
  observable,
  configure,
  runInAction,
} from 'mobx';
import { createContext, SyntheticEvent } from 'react';
import { toast } from 'react-toastify';
import { history } from '../..';
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

  @action loadActivities = () => {
    this.loadingInitial = true;
    agent.Activities.list()
      .then((activities) =>
        runInAction(() => {
          activities.forEach((activity) => {
            activity.date = new Date(activity.date);
            this.activityRegistry.set(activity.id, activity);
          });
        })
      )
      .finally(() => runInAction(() => (this.loadingInitial = false)));
  };
  
  @action loadActivity = async (id: string) => {
    let activity = this.getActivity(id);
    if (activity) {
      this.activity = activity;
      return activity;
    } else {
      this.loadingInitial = true;
      try {
        activity = await agent.Activities.details(id);
        runInAction(() => {
          activity.date = new Date(activity.date);
          this.activity = activity;
          this.activityRegistry.set(activity.id, activity);
          this.loadingInitial = false;
        });
        return activity;
      } catch (error) {
        runInAction(() => {
          this.loadingInitial = false;
        });
        console.log(error.response);
      }
    }
  };

  @action createActivity = async (activity: Activity) => {
    this.submitting = true;
    agent.Activities.create(activity)
      .then(() => {
        runInAction(() => {
          this.activityRegistry.set(activity.id, activity);
        });
        history.push(`/activities/${activity.id}`);
      })
      .catch((error) => {
        toast.error('Problem submitting data');
        console.error(error.response);
      })
      .finally(() => runInAction(() => (this.submitting = false)));
  };

  @action editActivity = async (activity: Activity) => {
    this.submitting = true;
    agent.Activities.update(activity)
      .then(() => {
        runInAction(() => {
          this.activityRegistry.set(activity.id, activity);
          this.activity = activity;
        });
        history.push(`/activities/${activity.id}`);
      })
      .catch((error) => {
        toast.error('Problem submitting data');
        console.error(error.response);
      })
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
  
  groupActivitiesByDate(activities: Activity[]) {
    const sortedActivities = activities.sort(
      (a, b) => a.date.getTime() - b.date.getTime()
    );

    return Object.entries(
      sortedActivities.reduce((activities, activity) => {
        const date = activity.date.toISOString().split('T')[0];
        activities[date] = activities[date]
          ? [...activities[date], activity]
          : [activity];
        return activities;
      }, {} as { [key: string]: Activity[] })
    );
  }
  
  getActivity = (id: string) => {
    return this.activityRegistry.get(id);
  };
}

export default createContext(new ActivityStore());
