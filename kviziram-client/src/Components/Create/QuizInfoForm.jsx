import { Card, Box, Typography, TextField, Button } from "@mui/material";
import React, { useState } from "react";
import SelectComponent from "./SelectComponent";

function QuizzInfoForm(props) {
  const [Category, setQuizzCategory] = useState(props.category);

  const categoryOptions = ['Option 1', 'Option 2', 'Option 3'];

  function handleCategoryChange(category)
  {
    setQuizzCategory(category);
  }


  return (
    <Box>
      <Card>
        <Typography variant="h5"> Quizz Name: </Typography>
        <TextField lable="Quizz Name" variant="outlined" value={props.quizzName} onChange={props.onQuizzNameChange}></TextField>
        <Typography variant="h6"> Category: </Typography>
        <SelectComponent options={categoryOptions} onChange={props.onCategoryChange}/>
      </Card>
    </Box>
  );
}

export default QuizzInfoForm;
