export const combineDateAndTime = (date: Date, time: Date) => {
    const dateString = `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
    const timeString = `${time.getHours()}:${time.getMinutes()}:00`;
    return new Date(dateString + ' ' + timeString);
}