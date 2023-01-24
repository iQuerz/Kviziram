function CalendarPicker(){
    return 
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
}