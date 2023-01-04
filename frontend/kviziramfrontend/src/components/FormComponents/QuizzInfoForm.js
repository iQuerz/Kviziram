import React, { useState } from "react";
import Card from "../Card";

function QuizzInfoForm() {
  const [quizzName, setQuizzName] = useState("");
  const [quizzCategory, setQuizzCategory] = useState("");

  function handleQuizzNameChange(event) {
    setQuizzName(event.target.value);
  }

  function handleQuizzCategoryChange(event) {
    setQuizzCategory(event.target.value);
  }

  return (
    <Card>
      <form>
        <label>
          Quizz Name:
          <input
            type="text"
            value={quizzName}
            onChange={handleQuizzNameChange}
          />
        </label>
        <br />
        <label>
          Quizz Category:
          <input
            type="text"
            value={quizzCategory}
            onChange={handleQuizzCategoryChange}
          />
        </label>
        <br />
      </form>
    </Card>
  );
}

export default QuizzInfoForm;
