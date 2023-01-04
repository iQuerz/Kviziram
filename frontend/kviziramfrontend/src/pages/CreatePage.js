import { useState } from "react";
import Card from "../components/Card";
import CreateQustion from "../components/FormComponents/CreateQuestion";
import QuizzInfoForm from "../components/FormComponents/QuizzInfoForm";
import "./CreatePage.modul.css";

function CreatePage() {
  const addUrl =
    "https://www.shutterstock.com/image-vector/plus-cross-symbols-hand-painted-260nw-1285469212.jpg";
  const [numOfQuestion, setNumOfQuestions] = useState(0);
  function handleAddQuizClick() {
    setNumOfQuestions(numOfQuestion + 1);
  }
  return (
    <Card>
        Create Page
        <QuizzInfoForm/>
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
      </div>
    </Card>
  );
}

export default CreatePage;
