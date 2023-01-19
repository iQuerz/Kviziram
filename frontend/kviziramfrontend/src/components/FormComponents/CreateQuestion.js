import { useState } from "react";

import Card from "../Card";

function CreateQustion(props) {
  const [question, setQuestion] = useState("");
  const [answers, setAnswers] = useState([
    { value: "", correct: false },
    { value: "", correct: false },
    { value: "", correct: false },
    { value: "", correct: false },
  ]);

  function handleQuestionChange(event) {
    setQuestion(event.target.value);
  }

  function handleAnswerChange(event, index) {
    const newAnswers = [...answers];
    newAnswers[index].value = event.target.value;
    setAnswers(newAnswers);
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
  }
  return (
    <Card>
        <div> Question number :{props.num + 1}</div>
      <form>
        <label>
          Question:
          <input type="text" value={question} onChange={handleQuestionChange} />
        </label>
        {answers.map((answer, index) => (
          <div key={index}>
            <label>
              Answer {index + 1}:
              <input
                type="text"
                value={answer.value}
                onChange={(event) => handleAnswerChange(event, index)}
              />
            </label>
            <input
              type="checkbox"
              checked={answer.correct}
              onChange={(event) => handleCorrectAnswerChange(event, index)}
            />
            Correct answer
          </div>
        ))}
      </form>
    </Card>
  );
}
export default CreateQustion;
