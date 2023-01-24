import { Card, Box, Typography, TextField, Button } from "@mui/material";
import React, { useState, useEffect } from "react";
import SelectComponent from "../Custom/SelectComponent";

function QuizzInfoForm(props) {
  const [category, setCategory] = useState(props.category);
  const [achievement, setAchievement] = useState(props.achievement);
  const [categoryOptions, setCategoryOptions] = useState([]);
  const [achievementOptions, setAchievementOptions] = useState([]);
  //const categoryOptions = ['Music', 'Sport', 'Gaming'];
  useEffect(() => {
    tryGetCategorys();
    tryGetAchivments();
  }, []);
  async function tryGetCategorys() {
    try {
      const response = await fetch("http://localhost:5221/Category/all", {
        method: "GET",
        headers: {
          accept: "text/plain",
        },
      });
      const json = await response.json();

      if (response.ok) {
        setCategoryOptions(json); //predaje se sessionID app komponenti
      }
    } catch (error) {
      console.error(error);
    }
  }
  async function tryGetAchivments() {
    try {
      const response = await fetch("http://localhost:5221/Achievement/all", {
        method: "GET",
        headers: {
          accept: "text/plain",
        },
      });
      const json = await response.json();

      if (response.ok) {
        setAchievementOptions(json); //predaje se sessionID app komponenti
      }
    } catch (error) {
      console.error(error);
    }
  }
  //nikola i made a mess sto se tice dizajna pls fix senpai ovi boxevi dole sve sam se izgubio da izgledaju lepo
  return (
    <Box className="margin">
      <Card className="flex-down seperate-children-small padding">
        <Typography variant="h5"> Quizz Name: </Typography>
        <TextField
          label="Quiz Name"
          variant="outlined"
          value={props.quizzName}
          onChange={props.onQuizzNameChange}
        ></TextField>

        <Box className="flex-down">
          <Box className="flex-right">
            <Typography variant="h6"> Category: </Typography>
            <Typography variant="h6"> Achievement: </Typography>
          </Box>
          <Box>
            {" "}
            <SelectComponent
              label="Category"
              options={categoryOptions}
              onChange={props.onCategoryChange}
            />
            <SelectComponent
              label="Achievement"
              options={achievementOptions}
              onChange={props.onAchivmentChange}
            />{" "}
          </Box>
        </Box>
      </Card>
    </Box>
  );
}

export default QuizzInfoForm;
