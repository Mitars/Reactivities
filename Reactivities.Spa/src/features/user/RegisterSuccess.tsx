import React from 'react';
import queryString from 'query-string';
import { Segment, Header, Icon, Button } from 'semantic-ui-react';
import agent from '../../app/api/agent';
import { toast } from 'react-toastify';
import { RouteComponentProps } from 'react-router-dom';

const RegisterSuccess = ({ location }: RouteComponentProps) => {
  const { email } = queryString.parse(location.search);
  const handleConfirmEmailResend = () => {
    agent.User.resendEmailVerification(email as string)
      .then(() =>
        toast.success('Verification email resent - please check you email')
      )
      .catch((error) => console.error(error));
  };

  return (
    <Segment placeholder>
      <Header icon>
        <Icon name="check" />
        Successfully registered!
      </Header>

      <Segment.Inline>
        <div className="center">
          <p>
            Please check your email (including junk folder) for the verification
            email
          </p>
          {email && (
            <>
              <p>
                Didn't receive the email? Please click the button below to
                resend
              </p>
              <Button
                onClick={handleConfirmEmailResend}
                primary
                content="Resend email"
                size="huge"
              />
            </>
          )}
        </div>
      </Segment.Inline>
    </Segment>
  );
};

export default RegisterSuccess;
