import React, { useState } from "react";
import Card from "../Card";

function QuizzInfoForm({ quizzInfo, setQuizzInfo }) {
  const [Name, setQuizzName] = useState(quizzInfo.name);
  const [Category, setQuizzCategory] = useState(quizzInfo.category);

  function handleQuizzNameChange(event) {
    setQuizzName(event.target.value);
    setQuizzInfo({ ...quizzInfo, name: event.target.value });
  }

  function handleQuizzCategoryChange(event) {
    setQuizzCategory(event.target.value);
    setQuizzInfo({ ...quizzInfo, category: event.target.value });
  }

  return (
    <Card>
      <form>
        <label>
          Quizz Name:
          <input
            type="text"
            value={Name}
            onChange={handleQuizzNameChange}
          />
        </label>
        <br />
        <label>
          Quizz Category:
          <input
            type="text"
            value={Category}
            onChange={handleQuizzCategoryChange}
          />
        </label>
        <br />
      </form>
    </Card>
  );
}

export default QuizzInfoForm;
