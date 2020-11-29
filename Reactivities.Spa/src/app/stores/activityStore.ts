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
  @observable activities: Activity[] = [];
  @observable selectedActivity: Activity | undefined;
  @observable loadingInitial = false;
  @observable editMode = false;
  @observable submitting = false;
  @observable target = '';

  @computed get activitiesByDate() {
    return Array.from(this.activityRegistry.values()).sort(
      (a, b) => Date.parse(a.date) - Date.parse(b.date)
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

  @action createActivity = (activity: Activity) => {
    this.submitting = true;
    agent.Activities.create(activity)
      .then(() =>
        runInAction(() => {
          this.activityRegistry.set(activity.id, activity);
          this.editMode = false;
        })
      )
      .finally(() => runInAction(() => (this.submitting = false)));
  };

  @action editActivity = (activity: Activity) => {
    this.submitting = true;
    agent.Activities.update(activity)
      .then(() =>
        runInAction(() => {
          this.activityRegistry.set(activity.id, activity);
          this.selectedActivity = activity;
          this.editMode = false;
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
          if (!this.activityRegistry.has(this.selectedActivity?.id)) {
            this.selectedActivity = undefined;
            this.editMode = false;
          }
        })
      )
      .finally(() => runInAction(() => (this.submitting = false)));
  };

  @action openCreateForm = () => {
    this.selectedActivity = undefined;
    this.editMode = true;
  };

  @action openEditForm = (id: string) => {
    this.selectedActivity = this.activityRegistry.get(id);
    this.editMode = true;
  };

  @action cancelSelectedActivity = () => {
    this.selectedActivity = undefined;
  };

  @action cancelFormOpen = () => {
    this.editMode = false;
  };

  @action selectActivity = (id: string) => {
    this.selectedActivity = this.activityRegistry.get(id);
    this.editMode = false;
  };
}

export default createContext(new ActivityStore());
