import {
  action,
  computed,
  observable,
  runInAction,
  makeObservable,
} from 'mobx';
import { User, UserFormValues } from '../models/user';
import agent from '../api/agent';
import { RootStore } from './rootStore';
import { history } from '../..';
import { toast } from 'react-toastify';

export default class UserStore {
  private refreshTokenTimeout: any;

  constructor(private rootStore: RootStore) {
    makeObservable(this, {
      user: observable,
      loading: observable,
      isLoggedIn: computed,
      login: action,
      register: action,
      getUser: action,
      logout: action,
      refreshToken: action,
    });
  }

  user: User | null = null;
  loading: boolean = false;

  get isLoggedIn() {
    return !!this.user;
  }

  login = async (values: UserFormValues) => {
    try {
      const user = await agent.User.login(values);
      runInAction(() => {
        this.user = user;
      });
      this.rootStore.commonStore.setToken(user.token);
      this.startRefreshTokenTimer(user);
      this.rootStore.modalStore.closeModal();
      history.push('/activities');
    } catch (error) {
      throw error;
    }
  };

  register = async (values: UserFormValues) => {
    try {
      await agent.User.register(values);
      this.rootStore.modalStore.closeModal();
      history.push(`/user/registerSuccess?email=${values.email}`);
    } catch (error) {
      throw error;
    }
  };

  getUser = async () => {
    try {
      const user = await agent.User.current();
      this.rootStore.commonStore.setToken(user.token);
      this.startRefreshTokenTimer(user);
      runInAction(() => {
        this.user = user;
      });
    } catch (error) {
      console.error(error);
    }
  };

  logout = () => {
    this.rootStore.commonStore.setToken(null);
    this.stopRefreshTokenTimer();
    this.user = null;
    history.push('/');
  };

  facebookLogin = (response: any) => {
    this.loading = true;
    agent.User.facebookLogin(response.accessToken)
      .then((user) =>
        runInAction(() => {
          this.user = user;
          this.rootStore.commonStore.setToken(user.token);
          this.startRefreshTokenTimer(user);
          this.rootStore.modalStore.closeModal();
          history.push('/activities');
        })
      )
      .catch((e) => console.error(e))
      .finally(() => runInAction(() => (this.loading = false)));
  };
  
  googleLogin = () => {
    this.loading = true;
  };

  googleLoginSuccess = (response: any) => {
    agent.User.googleLogin(response.accessToken)
      .then((user) =>
        runInAction(() => {
          this.user = user;
          this.rootStore.commonStore.setToken(user.token);
          this.startRefreshTokenTimer(user);
          this.rootStore.modalStore.closeModal();
          history.push('/activities');
        })
      )
      .catch((e) => {
        toast.error('Failed to login - please check the console for more info');
        console.error(e);
      })
      .finally(() => runInAction(() => (this.loading = false)));
  };
  
  googleLoginError = (response: any) => {
    this.loading = false;
    toast.error('Failed to login');
    console.error(response);
  };

  refreshToken = () => {
    this.stopRefreshTokenTimer();
    agent.User.refreshToken()
      .then((user) => {
        runInAction(() => (this.user = user));
        this.rootStore.commonStore.setToken(user.token);
        this.startRefreshTokenTimer(user);
      })
      .catch((e) => console.error(e));
  };

  private startRefreshTokenTimer(user: User) {
    const jwtToken = JSON.parse(atob(user.token.split('.')[1]));
    const expires = new Date(jwtToken.exp * 1_000);
    const timeout = expires.getTime() - Date.now() - 60 * 1_000;
    this.refreshTokenTimeout = setTimeout(this.refreshToken, timeout);
  }

  private stopRefreshTokenTimer() {
    clearTimeout(this.refreshTokenTimeout);
  }
}
