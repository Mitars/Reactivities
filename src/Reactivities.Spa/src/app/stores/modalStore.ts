import { action, observable, makeObservable } from 'mobx';
import { RootStore } from './rootStore';

export default class ModalStore {
  constructor(private rootStore: RootStore) {
    makeObservable(this, {
      modal: observable.shallow,
      openModal: action,
      closeModal: action
    });
  }

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
