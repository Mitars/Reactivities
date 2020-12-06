import React, { useContext, useEffect, useState } from 'react';
import { Button, Form, Grid, Segment } from 'semantic-ui-react';
import { Activity, ActivityFormValues } from '../../../app/models/activity';
import { v4 as uuid } from 'uuid';
import { observer } from 'mobx-react-lite';
import ActivityStore from '../../../app/stores/activityStore';
import { Link, RouteComponentProps } from 'react-router-dom';
import { Form as FinalForm, Field } from 'react-final-form';
import { TextInput } from '../../../app/common/form/TextInput';
import { TextAreaInput } from '../../../app/common/form/TextAreaInput';
import { SelectInput } from '../../../app/common/form/SelectInput';
import { DateInput } from '../../../app/common/form/DateInput';
import { category } from '../../../app/common/options/categoryOptions';
import { combineDateAndTime } from '../../../app/common/util/util';
import { combineValidators, composeValidators, hasLengthGreaterThan, isRequired } from 'revalidate';

const validate = combineValidators({
  title: isRequired({message: 'The event title is required'}),
  category: isRequired('Category'),
  description: composeValidators(
    isRequired('Description'),
    hasLengthGreaterThan(4)({message: 'Description needs to be at least 5 characters'})
  )(),
  city: isRequired('City'),
  venue: isRequired('Venue'),
  date: isRequired('Date'),
  time: isRequired('Time'),
});

const ActivityForm = ({
  match
}: RouteComponentProps<{ id: string }>) => {
  const activityStore = useContext(ActivityStore);
  const {
    createActivity,
    editActivity,
    submitting,
    loadActivity,
  } = activityStore;

  const [activity, setActivity] = useState(new ActivityFormValues());
  const [loading, setLoading] = useState(false);

  useEffect(() => {
      if (match.params.id) {
        setLoading(true);
        loadActivity(match.params.id).then(
          (activity: Activity) => setActivity(new ActivityFormValues(activity))
        ).finally(() => setLoading(false));
      }
    }, [
    loadActivity,
    match.params.id,
  ]);

  const handleFinalFormSubmit = (values: any) => {
    const dateAndTime = combineDateAndTime(values.date, values.time);
    const {date, time, ...activity} = values;
    activity.date = dateAndTime;
    
    
    if (!activity.id) {
      let newActivity = {
        ...activity,
        id: uuid(),
      };
      createActivity(newActivity);
    } else {
      editActivity(activity);
    }
  };

  return (
    <Grid>
      <Grid.Column width={10}>
        <Segment clearing>
          <FinalForm
            validate={validate}
            initialValues={activity}
            onSubmit={handleFinalFormSubmit}
            render={({ handleSubmit, invalid, pristine }) => (
              <Form onSubmit={handleSubmit} loading={loading}>
                <Field
                  name='title'
                  placeholder='Title'
                  value={activity.title}
                  component={TextInput}
                />
                <Field
                  component={TextAreaInput}
                  value={activity.description}
                  name='description'
                  rows={3}
                  placeholder='Description'
                />
                <Field
                  component={SelectInput}
                  value={activity.category}
                  options={category}
                  name='category'
                  placeholder='Category'
                />
                <Form.Group widths='equal'>
                  <Field
                    component={DateInput}
                    value={activity.date}
                    date={true}
                    name='date'
                    placeholder='Date'
                  />
                  <Field
                    component={DateInput}
                    value={activity.time}
                    time={true}
                    name='time'
                    placeholder='Time'
                  />
                </Form.Group>
                <Field
                  name='city'
                  placeholder='City'
                  value={activity.city}
                  component={TextInput}
                />
                <Field
                  name='venue'
                  placeholder='Venue'
                  value={activity.venue}
                  component={TextInput}
                />
                <Button
                  loading={submitting}
                  disabled={invalid || pristine}
                  positive
                  float='right'
                  type='submit'
                  content='Submit'
                ></Button>
                <Button
                  as={Link}
                  to={activity.id ? `/activities/${activity.id}`: '/activities'}
                  float='right'
                  type='submit'
                  content='Cancel'
                ></Button>
              </Form>
            )}
          />
        </Segment>
      </Grid.Column>
    </Grid>
  );
};

export default observer(ActivityForm);
