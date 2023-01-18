import { CheckBox } from "@mui/icons-material";
import { Box, TextField, Typography, Card, Button, Radio } from "@mui/material";
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
      <Card className="flex-down seperate-children-small padding">
        <Typography variant="h5"> Question {props.num + 1}:</Typography>
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
          <Box className="flex-down seperate-children-medium" sx={{border:1, borderRadius:2}}>
            <Typography>Question answers (mark the correct answer):</Typography>
            {answers.map((answer, index) => (
              <div key={index} className="flex-right">
                <TextField label = {"Answer " + (index + 1)}
                            value={answer.value}
                            onChange={(event) => handleAnswerChange(event, index)} />
                <Radio checked={answer.correct} name={"Question"+props.num}
                      label="Correct"
                      onChange={(event) => handleCorrectAnswerChange(event, index)}/>
              </div>
            ))}
            <Button variant="contained" size="large" onClick={handlenumOfAnswersChange}>+</Button>
          </Box>
      </Card>
    </Box>
  );
}
export default QuizzQuestions;
