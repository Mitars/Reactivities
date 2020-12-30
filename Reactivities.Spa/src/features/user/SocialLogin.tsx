import { observer } from 'mobx-react-lite';
import FacebookLogin from 'react-facebook-login/dist/facebook-login-render-props';
import { Button, Icon } from 'semantic-ui-react';

const SocialLogin = ({
  facebookCallback, loading
}: {
  facebookCallback: (response: any) => void;
  loading: boolean;
}) => {
  return (
    <div>
      <FacebookLogin
        appId='747933912821579'
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
    </div>
  );
};

export default observer(SocialLogin);
