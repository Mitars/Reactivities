import { action, computed, observable, runInAction, makeObservable } from 'mobx';
import { SyntheticEvent } from 'react';
import { toast } from 'react-toastify';
import { history } from '../..';
import agent from '../api/agent';
import { Activity } from '../models/activity';
import { RootStore } from './rootStore';

export default class ActivityStore {
  constructor(private rootStore: RootStore) {
    makeObservable(this, {
      activityRegistry: observable,
      activity: observable,
      loadingInitial: observable,
      submitting: observable,
      target: observable,
      activitiesByDate: computed,
      loadActivities: action,
      loadActivity: action,
      createActivity: action,
      editActivity: action,
      deleteActivity: action
    });
  }

  activityRegistry = new Map();
  activity: Activity | null = null;
  loadingInitial = false;
  submitting = false;
  target = '';

  get activitiesByDate() {
    return this.groupActivitiesByDate(
      Array.from(this.activityRegistry.values())
    );
  }

  loadActivities = () => {
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

  loadActivity = async (id: string) => {
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

  createActivity = async (activity: Activity) => {
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

  editActivity = async (activity: Activity) => {
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

  deleteActivity = (
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
