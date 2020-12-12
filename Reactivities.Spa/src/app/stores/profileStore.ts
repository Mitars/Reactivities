import {
  action,
  observable,
  computed,
  makeObservable,
  runInAction,
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
      uploadingPhoto: observable,
      loading: observable,
      loadProfile: action,
      uploadPhoto: action,
      setMainPhoto: action,
      deletePhoto: action,
      isCurrentUser: computed,
    });
  }

  profile: Profile | null = null;
  loadingProfile = true;
  uploadingPhoto = false;
  loading = false;

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

          this.uploadingPhoto = false;
        })
      )
      .catch((error) => {
        console.error(error);
        toast.error('Problem uploading photo');
      })
      .finally(() => runInAction(() => (this.uploadingPhoto = false)));
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
}
