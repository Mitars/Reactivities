import {
  action,
  observable,
  computed,
  makeObservable,
  runInAction,
  reaction,
} from 'mobx';
import { toast } from 'react-toastify';
import agent from '../api/agent';
import { Photo, Profile } from '../models/profile';
import { RootStore } from './rootStore';

export default class ProfileStore {
  constructor(private rootStore: RootStore) {
    makeObservable(this, {
      profile: observable,
      loadingProfile: observable,
      updatingProfile: observable,
      uploadingPhoto: observable,
      loading: observable,
      followings: observable,
      activeTab: observable,
      loadProfile: action,
      uploadPhoto: action,
      updateProfile: action,
      setMainPhoto: action,
      deletePhoto: action,
      follow: action,
      unfollow: action,
      loadFollowings: action,
      setActiveTab: action,
      isCurrentUser: computed,
    });

    reaction(
      () => this.activeTab,
      (activeTab) => {
        if (activeTab === 3 || activeTab === 4) {
          this.loadFollowings(activeTab === 3 ? 'followers' : 'following');
        } else {
          this.followings = [];
        }
      }
    );
  }

  profile: Profile | null = null;
  loadingProfile = true;
  uploadingPhoto = false;
  updatingProfile = false;
  loading = false;
  followings: Profile[] = [];
  activeTab: number = 0;

  get isCurrentUser() {
    if (this.rootStore.userStore.user && this.profile) {
      return this.rootStore.userStore.user.userName === this.profile.username;
    }

    return false;
  }

  loadProfile = (username: string) => {
    this.loadingProfile = true;
    agent.Profiles.get(username)
      .then((profile) => runInAction(() => (this.profile = profile)))
      .catch((error) => console.error(error))
      .finally(() => runInAction(() => (this.loadingProfile = false)));
  };

  uploadPhoto = async (file: Blob) => {
    this.uploadingPhoto = true;
    await agent.Profiles.uploadPhoto(file)
      .then((photo) =>
        runInAction(() => {
          if (this.profile) {
            this.profile.photos.push(photo);
            if (photo.isMain && this.rootStore.userStore.user) {
              this.rootStore.userStore.user.image = photo.url;
              this.profile.image = photo.url;
            }
          }
        })
      )
      .catch((error) => {
        console.error(error);
        toast.error('Problem uploading photo');
      })
      .finally(() => runInAction(() => (this.uploadingPhoto = false)));
  };

  updateProfile = async (profile: Partial<Profile>) => {
    this.updatingProfile = true;
    await agent.Profiles.update(profile)
      .then(() =>
        runInAction(() => {
          if (
            profile.displayName !== this.rootStore.userStore.user!.displayName
          ) {
            this.rootStore.userStore.user!.displayName = profile.displayName!;
          }

          this.profile = { ...this.profile!, ...profile };
        })
      )
      .catch((error) => {
        console.error(error);
        toast.error('Problem updating profile');
      })
      .finally(() => runInAction(() => (this.updatingProfile = false)));
  };

  setMainPhoto = (photo: Photo) => {
    this.loading = true;
    agent.Profiles.setMainPhoto(photo.id)
      .then(() =>
        runInAction(() => {
          this.rootStore.userStore.user!.image = photo.url;
          this.profile!.photos.find((p) => p.isMain)!.isMain = false;
          this.profile!.photos.find((p) => p.id === photo.id)!.isMain = true;
          this.profile!.image = photo.url;
        })
      )
      .catch(() => toast.error('Problem setting photo as main'))
      .finally(() => runInAction(() => (this.loading = false)));
  };

  deletePhoto = (photo: Photo) => {
    this.loading = true;
    agent.Profiles.deletePhoto(photo.id)
      .then(() =>
        runInAction(
          () =>
            (this.profile!.photos = this.profile!.photos.filter(
              (p) => p.id !== photo.id
            ))
        )
      )
      .catch(() => toast.error('Problem deleting the photo'))
      .finally(() => runInAction(() => (this.loading = false)));
  };

  modal = {
    open: false,
    body: null,
  };

  openModal = (contents: any) => {
    this.modal.open = true;
    this.modal.body = contents;
  };

  closeModal = () => {
    this.modal.open = false;
    this.modal.body = null;
  };

  follow = async (username: string) => {
    this.loading = true;
    agent.Profiles.follow(username)
      .then(() =>
        runInAction(() => {
          this.profile!.following = true;
          this.profile!.followersCount++;
        })
      )
      .catch(() => toast.error('Problem following user'))
      .finally(() => runInAction(() => (this.loading = false)));
  };

  unfollow = async (username: string) => {
    this.loading = true;
    agent.Profiles.unfollow(username)
      .then(() =>
        runInAction(() => {
          this.profile!.following = false;
          this.profile!.followersCount--;
        })
      )
      .catch(() => toast.error('Problem unfollowing user'))
      .finally(() => runInAction(() => (this.loading = false)));
  };

  loadFollowings = (predicate: string) => {
    this.loading = true;
    agent.Profiles.listFollowings(this.profile!.username, predicate)
      .then((profiles) => runInAction(() => (this.followings = profiles)))
      .catch(() => toast.error('Problem load followings'))
      .finally(() => runInAction(() => (this.loading = false)));
  };

  setActiveTab = (activeIndex: number) => (this.activeTab = activeIndex);

  setActiveTabReaction = () => {};
}
