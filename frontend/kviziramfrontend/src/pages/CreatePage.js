import { useState } from "react";
import Card from "../components/Card";
import CreateQustion from "../components/FormComponents/CreateQuestion";
import QuizzInfoForm from "../components/FormComponents/QuizzInfoForm";
import "./CreatePage.modul.css";

function CreatePage() {
  const addUrl =
    "https://www.shutterstock.com/image-vector/plus-cross-symbols-hand-painted-260nw-1285469212.jpg";
  const [numOfQuestion, setNumOfQuestions] = useState(0);
  const [quizzInfo, setQuizzInfo] = useState({ name: '', category: '' });
  const [questions, setQuestions] = useState([]);

  function handleAddQuizClick() {
    setNumOfQuestions(numOfQuestion + 1);
  }


  function handleSubmit(event) {
    event.preventDefault();

    // Create the request body
    const body = { quizzInfo, questions };
    console.log(body)
  }

  return (
    <Card>
        Create Page
        <QuizzInfoForm quizzInfo={quizzInfo} setQuizzInfo={setQuizzInfo}/>
      {Array.from({ length: numOfQuestion }).map((_, i) => (
        <CreateQustion key={i} num={i} />
      ))}
      <div>
        <img
          className="add"
          src={addUrl}
          alt="add"
          onClick={handleAddQuizClick}
        />
    <button onClick={handleSubmit}> Submit </button>
      </div>
    </Card>
  );
}

export default CreatePage;
