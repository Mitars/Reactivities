import {
  action,
  computed,
  observable,
  runInAction,
  makeObservable,
  toJS,
  reaction,
} from 'mobx';
import { SyntheticEvent } from 'react';
import { toast } from 'react-toastify';
import { history } from '../..';
import agent from '../api/agent';
import { createAttendee, setActivityProps } from '../common/util/util';
import { Activity } from '../models/activity';
import { RootStore } from './rootStore';
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from '@microsoft/signalr';

const LIMIT = 2;

export default class ActivityStore {
  constructor(private rootStore: RootStore) {
    makeObservable(this, {
      activityRegistry: observable,
      activity: observable,
      loadingInitial: observable,
      loading: observable,
      submitting: observable,
      target: observable,
      activityCount: observable,
      page: observable,
      predicate: observable,
      hubConnection: observable.ref,
      activitiesByDate: computed,
      totalPages: computed,
      loadActivities: action,
      loadActivity: action,
      createActivity: action,
      editActivity: action,
      deleteActivity: action,
      attendActivity: action,
      cancelAttendance: action,
      createHubConnection: action,
      stopHubConnection: action,
      addComment: action,
      setPage: action,
      setPredicate: action,
    });

    reaction(
      () => this.predicate.keys(),
      () => {
        this.page = 0;
        this.activityRegistry.clear();
        this.loadActivities();
      }
    );
  }

  activityRegistry = new Map();
  activity: Activity | null = null;
  loadingInitial = false;
  loading = false;
  submitting = false;
  target = '';
  hubConnection: HubConnection | null = null;
  activityCount: number = 0;
  page: number = 0;
  predicate = new Map();

  get activitiesByDate() {
    return this.groupActivitiesByDate(
      Array.from(this.activityRegistry.values())
    );
  }

  get totalPages() {
    return Math.ceil(this.activityCount / LIMIT);
  }

  loadActivities = async () => {
    this.loadingInitial = true;
    await agent.Activities.list(this.axiosParams)
      .then((activityList) =>
        runInAction(() => {
          if (this.rootStore.userStore.user === null) return;
          activityList.activities.forEach((activity) => {
            setActivityProps(activity, toJS(this.rootStore.userStore.user!));
            this.activityRegistry.set(activity.id, activity);
          });
          this.activityCount = activityList.activityCount;
        })
      )
      .finally(() => runInAction(() => (this.loadingInitial = false)));
  };

  loadActivity = async (id: string) => {
    this.loadingInitial = true;
    try {
      const activity = await agent.Activities.details(id);
      runInAction(() => {
        setActivityProps(activity, toJS(this.rootStore.userStore.user!));
        this.activity = activity;
        this.activityRegistry.set(activity.id, activity);
        this.loadingInitial = false;
      });
      return activity;
    } catch (error) {
      runInAction(() => {
        this.loadingInitial = false;
      });
    }
  };

  createActivity = async (activity: Activity) => {
    this.submitting = true;
    agent.Activities.create(activity)
      .then(() => {
        const attendee = createAttendee(toJS(this.rootStore.userStore.user!));
        attendee.isHost = true;
        activity.attendees = [attendee];
        activity.isHost = true;
        activity.comments = [];
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

  deleteActivity = (event: SyntheticEvent<HTMLButtonElement>, id: string) => {
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

  attendActivity = () => {
    const attendee = createAttendee(toJS(this.rootStore.userStore.user!));
    this.loading = true;
    agent.Activities.attend(this.activity!.id)
      .then(() =>
        runInAction(() => {
          if (this.activity) {
            this.activity.attendees.push(attendee);
            this.activity.isGoing = true;
            this.activityRegistry.set(this.activity.id, this.activity);
          }
        })
      )
      .catch(() => toast.error('Problem signing up to activity'))
      .finally(() => runInAction(() => (this.loading = false)));
  };

  cancelAttendance = () => {
    this.loading = true;
    agent.Activities.unattend(this.activity!.id)
      .then(() =>
        runInAction(() => {
          if (this.activity) {
            this.activity.attendees = this.activity.attendees.filter(
              (a) =>
                a.userName !== toJS(this.rootStore.userStore.user!.userName)
            );
            this.activity.isGoing = false;
            this.activityRegistry.set(this.activity.id, this.activity);
          }
        })
      )
      .catch(() => toast.error('Problem cancelling attendance'))
      .finally(() => runInAction(() => (this.loading = false)));
  };

  createHubConnection = (activityId: string) => {
    if (!this.hubConnection) {
      this.hubConnection = new HubConnectionBuilder()
        .withUrl('http://localhost:5000/chat', {
          accessTokenFactory: () => this.rootStore.commonStore.token!,
        })
        .configureLogging(LogLevel.Information)
        .build();

      this.hubConnection.on('ReceiveComment', (comment) =>
        runInAction(() => this.activity!.comments.push(comment))
      );
      this.hubConnection.on('Send', (message) => {
        /*toast.info(message)*/
      });
    }

    if (this.hubConnection!.state === 'Disconnected') {
      this.hubConnection
        .start()
        .then(() => this.hubConnection!.invoke('AddToGroup', activityId))
        .catch((error) =>
          console.error('Error establishing connection: ', error)
        );
    } else if (this.hubConnection!.state === 'Connected') {
      this.hubConnection!.invoke('AddToGroup', activityId);
    }
  };

  stopHubConnection = () => {
    if (this.hubConnection?.state === 'Connected') {
      this.hubConnection!.invoke('RemoveFromGroup', this.activity!.id)
        .then(() => this.hubConnection!.stop())
        .catch((error) => console.error(error));
    }
  };

  addComment = async (values: { activityId: string }) => {
    values.activityId = this.activity!.id;
    this.hubConnection!.invoke('SendComment', values).catch((error) =>
      console.error(error)
    );
  };

  setPage = (page: number) => (this.page = page);

  setPredicate = (predicate: string, value: string | Date) => {
    this.predicate.clear();
    if (predicate !== 'all') {
      this.predicate.set(predicate, value);
    }
  };

  @computed get axiosParams() {
    const params = new URLSearchParams();
    params.append('limit', String(LIMIT));
    params.append('offset', `${this.page ? this.page * LIMIT : 0}`);
    this.predicate.forEach((value, key) => {
      if (key === 'startDate') {
        params.append(key, value.toISOString());
      } else {
        params.append(key, value);
      }
    });
    return params;
  }
}
