import { PropaneSharp } from "@mui/icons-material";
import { Typography, Button } from "@mui/material";
import { Box } from "@mui/system";
import { useState } from "react";
import { Navigate } from "react-router";
import { useNavigate } from "react-router-dom";
import QuizzInfoForm from "../Components/Create/QuizInfoForm";
import QuizzQuestions from "../Components/Create/QuizzQuestions";
import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";

function CreateQuizPage(props) {
  const navigate = useNavigate();
  const [numOfQuestion, setNumOfQuestions] = useState(1);
  const [quizzName, setQuizzName] = useState("");
  const [category, setCategory] = useState("");
  const [achievement, setAchievement] = useState("");
  const [questions, setQuestions] = useState([
    {
      questionName: "",
      points: 0,
      allAnswers: [{ value: "", correct: false }],
    },
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
  function handleAchievementChange(event) {
    setAchievement(event);
  }
  function handleQuestionChange(question, index) {
    let newQuestions = [...questions];
    newQuestions[index] = question;
    setQuestions(newQuestions);
  }
  function handleSubmit() {
    console.log(props.mySessionID)
    let Quizz = formatQuizzForCreating();
    console.log(Quizz);
    tryCreate(Quizz);
  }
  //ne otvaraj niti ikada gledaj ovu funkciju dole
  function formatQuizzForCreating() {
    let Quizz = {
      name: quizzName,
      achievementID: achievement.id,
      categoryID: category.id,
      questions: [],
    };
    for (let question of questions) {
      let opetions = []
      var answeCounter = -1;
      var answerCorrectIndex = 0;
      for (let answer of question[0].allAnswers) {
        answeCounter++;
        opetions.push(answer.value);
        if (answer.correct == true) answerCorrectIndex = answeCounter;
      }
      Quizz.questions.push({
        description: question[0].questionName,
        options: opetions,
        answer: answerCorrectIndex,
        points: question[0].questionPoints,
      });
    }
    return Quizz;
  }
  async function tryCreate(Quizz)
    {   
        try {
          console.log(Quizz)
          const endpoint = "http://localhost:5221/Quiz"; // replace with the actual endpoint URL
          const body = JSON.stringify(Quizz);
          const response = await fetch(endpoint, {
            method: "POST",
            body: body,
            headers: { "Content-Type": "application/json",
                      'SessionID':  props.mySessionID}
          });
          const json = await response.json();
          if (!response.ok) {
            throw json;
          }
          if(json)
          {
            console.log(json)
            handleFetchSuccess()
          }
        } catch (error) {
          throw error;
        }     
    }  
    function handleFetchSuccess()
    {
      //to do reset everything or something
      console.log("to be implemented")
      navigate('/Play')
    }
  return (
    <>
      <SidebarLayout sessionID={props.mySessionID}>
        <QuizzInfoForm
          quizzName={quizzName}
          onQuizzNameChange={handleQuizzNameChange}
          quizzCategory={category}
          onCategoryChange={handleCategoryChange}
          quizzAchievement={achievement}
          onAchivmentChange={handleAchievementChange}
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
        <Button variant="contained" size="large" onClick={handleSubmit}>
          Finish!
        </Button>
      </SidebarLayout>
    </>
  );
}

export default CreateQuizPage;
