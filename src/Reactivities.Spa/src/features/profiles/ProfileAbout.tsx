import React, { useContext, useState } from 'react';
import { observer } from 'mobx-react-lite';
import { Button, Form, Grid, Header, Tab } from 'semantic-ui-react';
import { Form as FinalForm, Field } from 'react-final-form';
import { TextInput } from '../../app/common/form/TextInput';
import { TextAreaInput } from '../../app/common/form/TextAreaInput';
import { combineValidators, isRequired } from 'revalidate';
import { RootStoreContext } from '../../app/stores/rootStore';
import { Profile } from '../../app/models/profile';

const validate = combineValidators({
  displayName: isRequired('Display Name'),
});

const ProfileAbout = () => {
  const rootStore = useContext(RootStoreContext);
  const {
    isCurrentUser,
    profile,
    updateProfile,
    updatingProfile,
  } = rootStore.profileStore;
  const [editProfileMode, setEditProfileMode] = useState(false);

  return (
    <Tab.Pane>
      <Grid>
        <Grid.Column width={16}>
          <Header
            floated={'left'}
            icon={'user'}
            content={`About ${profile?.displayName}`}
          />
          {isCurrentUser &&
            (editProfileMode ? (
              <Button
                floated={'right'}
                basic
                content={'Cancel'}
                onClick={() => setEditProfileMode(false)}
              />
            ) : (
              <Button
                floated={'right'}
                basic
                content={'Edit Profile'}
                onClick={() => setEditProfileMode(true)}
              />
            ))}
        </Grid.Column>
        <Grid.Column width={16}>
          {editProfileMode ? (
            <>
              <FinalForm
                validate={validate}
                initialValues={profile}
                onSubmit={(values: Partial<Profile>) => {
                  values.bio = values.bio ?? '';
                  updateProfile(values).then(() => setEditProfileMode(false));
                }}
                render={({ handleSubmit, invalid, pristine }) => (
                  <Form onSubmit={handleSubmit}>
                    <Field
                      name="displayName"
                      placeholder="Display Name"
                      value={profile!.displayName}
                      component={TextInput}
                    />
                    <Field
                      name="bio"
                      placeholder="Bio"
                      rows={3}
                      value={profile!.bio}
                      component={TextAreaInput}
                    />
                    <Button
                      loading={updatingProfile}
                      disabled={invalid || pristine || updatingProfile}
                      floated="right"
                      positive
                      type="submit"
                      content={'Update Profile'}
                    />
                  </Form>
                )}
              />
            </>
          ) : (
            <>{profile!.bio}</>
          )}
        </Grid.Column>
      </Grid>
    </Tab.Pane>
  );
};

export default observer(ProfileAbout);
