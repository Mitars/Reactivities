import React from 'react';
import { Dimmer, Loader } from 'semantic-ui-react';

interface IProp {
  inverted?: boolean;
  content?: string;
}

export const LoadingComponent = ({ inverted = true, content }: IProp) => {
  return (
    <Dimmer active inverted={inverted}>
      <Loader content={content} />
    </Dimmer>
  );
};
