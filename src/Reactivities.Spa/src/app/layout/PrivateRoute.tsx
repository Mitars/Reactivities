import { observer } from 'mobx-react-lite';
import React, { useContext } from 'react';
import {
  RouteProps,
  RouteComponentProps,
  Route,
  Redirect,
} from 'react-router-dom';
import { RootStoreContext } from '../stores/rootStore';

interface Props extends RouteProps {
  component: React.ComponentType<RouteComponentProps<any>>;
}

const PrivateRoute = ({ component: Component, ...rest }: Props) => {
  const rootStore = useContext(RootStoreContext);
  const { isLoggedIn } = rootStore.userStore;

  return (
    <Route
      {...rest}
      render={(props) =>
        isLoggedIn ? <Component {...props} /> : <Redirect to={'/'} />
      }
    />
  );
};

export default observer(PrivateRoute);
