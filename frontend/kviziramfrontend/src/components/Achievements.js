import React from 'react';
import './Achievements.css';

function Achievements(props) {
  const { achievements } = props;

  return (
    <div className="achievements">
      <h1>Achievements</h1>
      <ul>
        {achievements.map((achievement) => (
          <li key={achievement.id}>
            <img src={achievement.iconUrl} alt={achievement.name} />
            <div>
              <h2>{achievement.name}</h2>
              <p>{achievement.description}</p>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default Achievements;