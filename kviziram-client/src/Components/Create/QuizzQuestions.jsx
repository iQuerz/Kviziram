import { Box, TextField, Typography, Card, Button } from "@mui/material";
import { useState } from "react";

function QuizzQuestions(props) {
  const [question, setQuestion] = useState("");
  const [numOfAnswers, setNumOfAnswers] = useState(1);
  const [answers, setAnswers] = useState([{ value: "", correct: false }]);
  const [points, setPoints] = useState(0);

  function handlenumOfAnswersChange(){
    setNumOfAnswers(numOfAnswers + 1);
    setAnswers(answers.concat({ value: "", correct: false }));
  }

  function handleAnswerChange(event, index){
    const newAnswers = [...answers];
    newAnswers[index].value = event.target.value;
    setAnswers(newAnswers);
    sendQuestionToParentPage();
  }
  function handleQuestionChange(event, index){
    setQuestion(event.target.value);
    sendQuestionToParentPage();
  }
  function handleCorrectAnswerChange(event, index) {
    const newAnswers = [...answers];
    newAnswers.forEach((answer, i) => {
      if (i === index) {
        answer.correct = true;
      } else {
        answer.correct = false;
      }
    });
    setAnswers(newAnswers);
    sendQuestionToParentPage();
  }
  function handlePointsChange(event)
  {
    setPoints(event.target.value)
    sendQuestionToParentPage();
  }
  function sendQuestionToParentPage(){
    props.onQuestionChange([{questionName: question, questionPoints: points, allAnswers: answers}], props.num)
  }


  return (
    <Box className="margin">
      <Card className="flex-down seperate-children-small">
        <Typography variant="h5"> Question number : {props.num + 1}</Typography>
            <TextField multiline
              label="Question"
              value={props.question}
              onChange={handleQuestionChange}
            />
            <TextField
              label="Points"
              type="number"
              value={props.points}
              onChange={handlePointsChange}
            />
          {answers.map((answer, index) => (
            <div key={index}>
              <Typography  variant="h7">
                Answer {index + 1}:
                <TextField
                  value={answer.value}
                  onChange={(event) => handleAnswerChange(event, index)}
                />
              </Typography>
              <input
                type="checkbox"
                checked={answer.correct}
                onChange={(event) => handleCorrectAnswerChange(event, index)}
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
