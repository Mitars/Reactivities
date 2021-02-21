import { observer } from 'mobx-react-lite';
import React from 'react';
import FacebookLogin from 'react-facebook-login/dist/facebook-login-render-props';
import { GoogleLogin } from 'react-google-login';
import { Button, Container, Icon } from 'semantic-ui-react';

const SocialLogin = ({
  facebookCallback, googleCallback, googleCallbackSuccess, googleCallbackError, loading
}: {
  facebookCallback: (response: any) => void;
  googleCallback: () => void;
  googleCallbackSuccess: (response: any) => void;
  googleCallbackError: (response: any) => void;
  loading: boolean;
}) => {
  return (
    <div>
      <FacebookLogin
        appId={process.env.REACT_APP_FACEBOOK_ID ?? ''}
        fields='name,email,picture'
        callback={facebookCallback}
        render={(renderProps: any) => (
          <Button
            loading={loading}
            onClick={renderProps.onClick}
            type='button'
            fluid
            color='facebook'
          >
            <Icon name='facebook' />
            Login with Facebook
          </Button>
        )}
      />
      <Container style={{ marginTop: '10px' }}></Container>
      <GoogleLogin
        clientId={process.env.REACT_APP_GOOGLE_ID ?? ''}
        buttonText="Login"
        onRequest={googleCallback}
        onSuccess={googleCallbackSuccess}
        onFailure={googleCallbackError}
        render={(renderProps: any) => (
          <Button
            loading={loading}
            onClick={renderProps.onClick}
            type='button'
            fluid
            color='google plus'
          >
            <Icon name='google' />
            Login with Google
          </Button>
        )}
        cookiePolicy={'single_host_origin'}
      />
    </div>
  );
};

export default observer(SocialLogin);
