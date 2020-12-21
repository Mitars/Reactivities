import React from 'react';
import { Tab } from 'semantic-ui-react';
import ProfilePhotos from './ProfilePhotos';
import ProfileAbout from './ProfileAbout';
import ProfileFollowing from './ProfileFollowing';

const panes = [
  { menuItem: 'About', render: () => <ProfileAbout /> },
  { menuItem: 'Photos', render: () => <ProfilePhotos /> },
  {
    menuItem: 'Activities',
    render: () => <Tab.Pane>Activities content</Tab.Pane>,
  },
  {
    menuItem: 'Followers',
    render: () => <ProfileFollowing />,
  },
  {
    menuItem: 'Following',
    render: () => <ProfileFollowing />,
  },
];

export const ProfileContent = ({
  setActiveTab,
}: {
  setActiveTab: (activeIndex: number) => void;
}) => {
  return (
    <Tab
      menu={{ fluid: true, vertical: true }}
      menuPosition="right"
      panes={panes}
      onTabChange={(e, data) => setActiveTab(data.activeIndex as number)}
    />
  );
};
