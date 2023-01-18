import { Typography, Button } from "@mui/material";
import { Box } from "@mui/system";
import { useState } from "react";
import QuizzInfoForm from "../Components/Create/QuizInfoForm";
import QuizzQuestions from "../Components/Create/QuizzQuestions";
import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";

function CreateQuizPage() {
  const [numOfQuestion, setNumOfQuestions] = useState(1);
  const [quizzName, setQuizzName] = useState("");
  const [category, setCategory] = useState("");
  const [questions, setQuestions] = useState([
    { questionName: "", points: 0, allAnswers: [{ value: "", correct: false }] },
  ]);

  function handleNumOfQuestionsChange() {
    setNumOfQuestions(numOfQuestion + 1);
    setQuestions(
      questions.concat([
        {
          questionName: questions,
          points: 0,
          allAnswers: [{ value: "", correct: false }],
        },
      ])
    );
  }
  function handleQuizzNameChange(event) {
    setQuizzName(event.target.value);
  }
  function handleCategoryChange(event) {
    setCategory(event);
  }
  function handleQuestionChange(question, index) {
    let newQuestions = [...questions];
    newQuestions[index] = question;
    setQuestions(newQuestions);
  }
  function printQuizz()
  {
    console.log(questions);
  }
  return (
    <>
      <SidebarLayout>
        <Typography></Typography>
        <QuizzInfoForm
          quizzName={quizzName}
          onQuizzNameChange={handleQuizzNameChange}
          quizzCategory={category}
          onCategoryChange={handleCategoryChange}
        ></QuizzInfoForm>

        <Box className="flex-list-row">
          {Array.from({ length: numOfQuestion }).map((_, i) => (
            <QuizzQuestions
              key={i}
              num={i}
              onQuestionChange={handleQuestionChange}
            />
          ))}

          <Button
            variant="contained"
            size="large"
            onClick={handleNumOfQuestionsChange}
          >
            +
          </Button>
        </Box>
        
      </SidebarLayout>
    </>
  );
}

export default CreateQuizPage;
