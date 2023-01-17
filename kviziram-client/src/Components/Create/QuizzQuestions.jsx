import { Box, TextField, Typography, Card, Button } from "@mui/material";
import { useState } from "react";

function QuizzQuestions(props) {
  const [question, setQuestion] = useState("");
  const [numOfAnswers, setNumOfAnswers] = useState(1);
  const [answers, setAnswers] = useState([{ value: "", correct: false }]);

  function handlenumOfAnswersChange(){
    setNumOfAnswers(numOfAnswers + 1);
    setAnswers(answers.concat({ value: "", correct: false }));
  }

  return (
    <Box>
      <Card>
        <Typography variant="h5"> Question number : {props.num + 1}</Typography>
          <Typography variant="h6">
            Question:
            <TextField
              value={question}
              onChange={props.handleQuestionChange}
            />
          </Typography>
          {answers.map((answer, index) => (
            <div key={index}>
              <Typography  variant="h7">
                Answer {index + 1}:
                <TextField
                  value={answer.value}
                  onChange={(event) => props.handleAnswerChange(event, index)}
                />
              </Typography>
              <input
                type="checkbox"
                checked={answer.correct}
                onChange={(event) => props.handleCorrectAnswerChange(event, index)}
              />
              Correct answer
            </div>
          ))}
          <Button variant="contained" size="large" onClick={handlenumOfAnswersChange}>+</Button>
      </Card>
    </Box>
  );
}
export default QuizzQuestions;
