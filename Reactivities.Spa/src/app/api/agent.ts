import axios, { AxiosResponse } from 'axios';
import { history } from '../..';
import { Activity } from '../models/activity';
import { toast } from 'react-toastify';

axios.defaults.baseURL = 'http://localhost:5000/api';

axios.interceptors.response.use(
  (response) => sleep(0)(response),
  (error) => {
    if (error.message === 'Network Error' && !error.response) {
      toast.error('Network error - make sure API is running!');
    }

    console.log(error);
    const { status, data, config } = error.response;
    if (status === 404) {
      history.push('/notfound');
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
  }
);

const responseBody = (response: AxiosResponse) => response.data;

const sleep = (ms: number) => (response: AxiosResponse) =>
  new Promise<AxiosResponse>((resolve) =>
    setTimeout(() => resolve(response), ms)
  );

const requests = {
  get: (url: string) => axios.get(url).then(responseBody),
  post: (url: string, body: {}) => axios.post(url, body).then(responseBody),
  put: (url: string, body: {}) => axios.put(url, body).then(responseBody),
  del: (url: string) => axios.delete(url).then(responseBody),
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
