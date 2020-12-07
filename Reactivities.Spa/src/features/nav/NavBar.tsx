import { observer } from 'mobx-react-lite';
import React, { useContext } from 'react';
import { Link, NavLink } from 'react-router-dom';
import { Button, Container, Dropdown, Image, Menu } from 'semantic-ui-react';
import { RootStoreContext } from '../../app/stores/rootStore';

const NavBar = () => {
  const rootStore = useContext(RootStoreContext);
  const { user, logout } = rootStore.userStore;

  console.log(user);
  return (
    <Menu fixed="top" inverted>
      <Container>
        <Menu.Item header exact as={NavLink} to="/">
          <img
            src="/assets/logo.png"
            alt="logo"
            style={{ marginRight: '10px' }}
          />
          Reactivities
        </Menu.Item>
        <Menu.Item name="Activities" as={NavLink} to="/activities" />
        <Menu.Item>
          <Button
            positive
            content="Create Activity"
            as={NavLink}
            to="/createActivity"
          />
        </Menu.Item>
        {user && (
          <Menu.Item position="right">
            <Image
              avatar
              spaced="right"
              src={user.image || '/assets/user.png'}
            />
            <Dropdown pointing="top left" text={user.displayName}>
              <Dropdown.Menu>
                <Dropdown.Item
                  as={Link}
                  to={`/profile/username`}
                  text="My profile"
                  icon="user"
                />
                <Dropdown.Item text="Logout" icon="power" onClick={logout} />
              </Dropdown.Menu>
            </Dropdown>
          </Menu.Item>
        )}
      </Container>
    </Menu>
  );
};

export default observer(NavBar);
