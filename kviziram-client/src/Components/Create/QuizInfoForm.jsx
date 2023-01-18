import { Card, Box, Typography, TextField, Button } from "@mui/material";
import React, { useState } from "react";
import SelectComponent from "./SelectComponent";

function QuizzInfoForm(props) {
  const [Category, setQuizzCategory] = useState(props.category);

  const categoryOptions = ['Music', 'Sport', 'Gaming'];

  function handleCategoryChange(category)
  {
    setQuizzCategory(category);
  }


  return (
    <Box className="margin">
      <Card className="flex-down seperate-children-small padding">
        <Typography variant="h5"> Quizz Name: </Typography>
        <TextField label="Quiz Name" variant="outlined" value={props.quizzName} onChange={props.onQuizzNameChange}></TextField>
        <Typography variant="h6"> Category: </Typography>
        <SelectComponent label="Category" options={categoryOptions} onChange={props.onCategoryChange}/>
      </Card>
    </Box>
  );
}

export default QuizzInfoForm;
