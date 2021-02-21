import React from 'react';
import { FieldRenderProps } from 'react-final-form';
import { FormFieldProps, Form, Label } from 'semantic-ui-react';
import { DateTimePicker } from 'react-widgets';

interface IProps
  extends FieldRenderProps<Date, HTMLInputElement>,
    FormFieldProps {}

export const DateInput = ({
  input,
  width,
  date = false,
  time = false,
  placeholder,
  meta: { touched, error },
}: IProps) => {
  return (
    <Form.Field error={touched && !!error} width={width}>
      <DateTimePicker
        value={input.value || null}
        placeholder={placeholder}
        date={date}
        time={time}
        onKeyDown={(e) => e.preventDefault()}
        onChange={input.onChange}
        onBlur={input.onBlur}
      />
      {touched && error && (
        <Label basic color='red'>
          {error}
        </Label>
      )}
    </Form.Field>
  );
};
