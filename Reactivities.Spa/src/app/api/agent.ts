import axios, { AxiosResponse } from 'axios';
import { history } from '../..';
import { Activity, ActivityList } from '../models/activity';
import { toast } from 'react-toastify';
import { User, UserFormValues } from '../models/user';
import { Photo, Profile } from '../models/profile';

axios.defaults.baseURL = 'http://localhost:5000/api';

axios.interceptors.request.use(
  (config) => {
    const token = window.localStorage.getItem('jwt');
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  },
  (error) => Promise.reject(error)
);

axios.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.message === 'Network Error' && !error.response) {
      toast.error('Network error - make sure API is running!');
    }

    //console.error(error);
    const { status, data, config, headers } = error.response;
    if (status === 404) {
      history.push('/notfound');
    }

    if (
      status === 401 &&
      headers['www-authenticate'].includes(
        'Bearer error="invalid_token", error_description="The token expired'
      )
    ) {
      window.localStorage.removeItem('jwt');
      history.push('/');
      toast.info('Your session has expired, please login again');
    }

    if (
      status === 400 &&
      config.method === 'get' &&
      data.errors.hasOwnProperty('id')
    ) {
      history.push('/notfound');
    }

    if (status === 500) {
      toast.error('Server error - check the terminal for more info!');
    }

    throw error.response;
  }
);

const responseBody = (response: AxiosResponse) => response.data;

const requests = {
  get: (url: string) => axios.get(url).then(responseBody),
  post: (url: string, body: {}) => axios.post(url, body).then(responseBody),
  put: (url: string, body: {}) => axios.put(url, body).then(responseBody),
  del: (url: string) => axios.delete(url).then(responseBody),
  postForm: (url: string, file: Blob) => {
    let formData = new FormData();
    formData.append('File', file);
    return axios
      .post(url, formData, {
        headers: { 'Content-type': 'multipart/form-data' },
      })
      .then(responseBody);
  },
};

const Activities = {
  list: (params: URLSearchParams): Promise<ActivityList> =>
    axios.get(`/activities`, { params: params }).then(responseBody),
  details: (id: string) => requests.get(`/activities/${id}`),
  create: (activity: Activity) => requests.post('/activities', activity),
  update: (activity: Activity) =>
    requests.put(`/activities/${activity.id}`, activity),
  delete: (id: string) => requests.del(`/activities/${id}`),
  attend: (id: string) => requests.post(`/activities/${id}/attend`, {}),
  unattend: (id: string) => requests.del(`/activities/${id}/attend`),
};

const UserAgent = {
  current: (): Promise<User> => requests.get('/user'),
  login: (user: UserFormValues): Promise<User> =>
    requests.post('/user/login', user),
  register: (user: UserFormValues): Promise<User> =>
    requests.post('/user/register', user),
};

const Profiles = {
  get: (username: string): Promise<Profile> =>
    requests.get(`/profiles/${username}`),
  update: (profile: Partial<Profile>) => requests.put(`/profiles`, profile),
  uploadPhoto: (photo: Blob): Promise<Photo> =>
    requests.postForm(`/photos`, photo),
  setMainPhoto: (id: string) => requests.post(`/photos/${id}/setMain`, {}),
  deletePhoto: (id: string) => requests.del(`/photos/${id}`),
  follow: (username: string) =>
    requests.post(`/profiles/${username}/follow`, {}),
  unfollow: (username: string) => requests.del(`/profiles/${username}/follow`),
  listFollowings: (username: string, predicate: string) =>
    requests.get(`/profiles/${username}/follow?predicate=${predicate}`),
  listActivities: (username: string, predicate: string) =>
    requests.get(`profiles/${username}/activities?predicate=${predicate}`),
};

export default {
  Activities,
  User: UserAgent,
  Profiles,
};
