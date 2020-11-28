import axios, { AxiosResponse } from 'axios';
import { Activity } from '../models/activity';

axios.defaults.baseURL = 'http://localhost:5000/api';

const responseBody = (response: AxiosResponse) => response.data;

const sleep = (ms: number) => (response: AxiosResponse) =>
  new Promise<AxiosResponse>((resolve) =>
    setTimeout(() => resolve(response), ms)
  );

const intercept = (response: AxiosResponse) => sleep(1000)(response);

const requests = {
  get: (url: string) => axios.get(url).then(intercept).then(responseBody),
  post: (url: string, body: {}) =>
    axios.post(url, body).then(intercept).then(responseBody),
  put: (url: string, body: {}) =>
    axios.put(url, body).then(intercept).then(responseBody),
  del: (url: string) => axios.delete(url).then(intercept).then(responseBody),
};

const Activities = {
  list: (): Promise<Activity[]> => requests.get('/activities'),
  details: (id: string) => requests.get(`/activities/${id}`),
  create: (activity: Activity) => requests.post('/activities', activity),
  update: (activity: Activity) =>
    requests.put(`/activities/${activity.id}`, activity),
  delete: (id: string) => requests.del(`/activities/${id}`),
};

export default {
  Activities,
};
