import { action, computed, observable, runInAction, makeObservable } from 'mobx';
import { User, UserFormValues } from '../models/user';
import agent from '../api/agent';
import { RootStore } from './rootStore';
import { history } from '../..';

export default class UserStore {
  constructor(private rootStore: RootStore) {
    makeObservable(this, {
      user: observable,
      isLoggedIn: computed,
      login: action,
      register: action,
      getUser: action,
      logout: action
    });
  }

  user: User | null = null;

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
      this.rootStore.modalStore.closeModal();
      history.push('/activities');
    } catch (error) {
      throw error;
    }
  };

  register = async (values: UserFormValues) => {
    try {
      const user = await agent.User.register(values);
      this.rootStore.commonStore.setToken(user.token);
      this.rootStore.modalStore.closeModal();
      history.push('/activities');
    } catch (error) {
      throw error;
    }
  };

  getUser = async () => {
    try {
      const user = await agent.User.current();
      runInAction(() => {
        this.user = user;
      });
    } catch (error) {
      console.error(error);
    }
  };

  logout = () => {
    this.rootStore.commonStore.setToken(null);
    this.user = null;
    history.push('/');
  };
}
