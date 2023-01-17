import { Typography, Button } from "@mui/material";
import { useState } from "react";
import QuizzInfoForm from "../Components/Create/QuizInfoForm";
import QuizzQuestions from "../Components/Create/QuizzQuestions";
import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";

function CreateQuizPage() {

    const [numOfQuestion, setNumOfQuestions] = useState(1);
    const [quizzName, setQuizzName] = useState("");
    const [category, setCategory] = useState("");
    const [questions, setQuestions] = useState([]);

    function handleNumOfQuestionsChange(){
        setNumOfQuestions(numOfQuestion + 1)
    }
    function handleQuizzNameChange(event){
        setQuizzName(event.target.value)
    }
    function handleCategoryChange(event)
    {
        setCategory(event)
    }
    function handleQuestionChange()
    {

    }
    return(
        <>
            <SidebarLayout>
                <Typography></Typography>
                <QuizzInfoForm 
                quizzName={quizzName} onQuizzNameChange={handleQuizzNameChange} 
                quizzCategory={category} onCategoryChange={handleCategoryChange}></QuizzInfoForm>
                {Array.from({ length: numOfQuestion }).map((_, i) => (
                <QuizzQuestions key={i} num={i} />
                ))}
                <Button variant="contained" size="large" onClick={handleNumOfQuestionsChange}>+</Button>

            </SidebarLayout>
        </>
    )
}

export default CreateQuizPage