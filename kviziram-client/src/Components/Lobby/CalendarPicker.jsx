function CalendarPicker() {
  function handleDatePickerChange(event) {
    const [fromDate, setfromDate] = useState(dayjs("2014-08-18T21:11:54"));
    const [toDate, settoDate] = useState(dayjs("2014-08-18T21:11:54"));
  
    const handleFromDateChange = (newValue) => {
      setfromDate(newValue);
    };
  
  
    const handleToDateChange = (newValue) => {
      settoDate(newValue);
    };
  }
  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <Typography fontWeight={"bold"} p={1}>
        From:
      </Typography>
      <DesktopDatePicker
        label="Date from"
        inputFormat="MM/DD/YYYY"
        value={fromDate}
        onChange={handleFromDateChange}
        renderInput={(params) => <TextField {...params} />}
      />
      <Typography fontWeight={"bold"} p={1}>
        to:
      </Typography>

      <DesktopDatePicker
        label="Date from"
        inputFormat="MM/DD/YYYY"
        value={toDate}
        onChange={handleToDateChange}
        renderInput={(params) => <TextField {...params} />}
      />
    </LocalizationProvider>
  );
}
export default CalendarPicker;
