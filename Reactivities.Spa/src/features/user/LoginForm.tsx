import { FORM_ERROR } from 'final-form';
import React, { useContext } from 'react';
import { Form as FinalForm, Field } from 'react-final-form';
import { combineValidators, isRequired } from 'revalidate';
import { Button, Divider, Form, Header } from 'semantic-ui-react';
import { TextInput } from '../../app/common/form/TextInput';
import { UserFormValues } from '../../app/models/user';
import { RootStoreContext } from '../../app/stores/rootStore';
import { ErrorMessage } from '../../app/common/form/ErrorMessage';
import SocialLogin from './SocialLogin';
import { observer } from 'mobx-react-lite';

const validate = combineValidators({
  email: isRequired('email'),
  password: isRequired('password'),
});

const LoginForm = () => {
  const rootStore = useContext(RootStoreContext);
  const { login, facebookLogin, loading } = rootStore.userStore;

  return (
    <FinalForm
      onSubmit={(values: UserFormValues) =>
        login(values).catch((error) => ({
          [FORM_ERROR]: error,
        }))
      }
      validate={validate}
      render={({
        handleSubmit,
        submitting,
        submitError,
        invalid,
        pristine,
        dirtySinceLastSubmit,
      }) => (
        <Form onSubmit={handleSubmit} error>
          <Header
            as="h2"
            content="Login to Reactivities"
            color="teal"
            textAlign="center"
          />
          <Field name="email" component={TextInput} placeholder="Email" />
          <Field
            name="password"
            component={TextInput}
            placeholder="Password"
            type="password"
            autoComplete="current-password"
          />
          {submitError && !dirtySinceLastSubmit && (
            <ErrorMessage
              error={submitError}
              text="Invalid username or password"
            />
          )}
          <Button
            disabled={(!dirtySinceLastSubmit && invalid) || pristine}
            loading={submitting}
            color="teal"
            content="Login"
            fluid
          />
          <Divider horizontal>Or</Divider>
          <SocialLogin loading={loading} facebookCallback={facebookLogin} />
        </Form>
      )}
    />
  );
};

export default observer(LoginForm);