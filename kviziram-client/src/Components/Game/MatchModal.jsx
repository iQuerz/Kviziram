import {
  Box,
  Button,
  Card,
  CardHeader,
  MenuItem,
  Modal,
  Select,
  TextField,
  Typography,
} from "@mui/material";

import { useState } from "react";
import { useEffect } from "react";

const style = {
  position: "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: "50%",
  bgcolor: "var(--white)",
  boxShadow: 24,
  p: 4,
};
var matchDate = "";

function MatchModal(props) {
  const [match, setMatch] = useState("");
  const [winerName, setWinerName] = useState("");
  const [quiz, setQuiz] = useState({});
  const [scores, setScores] = useState([]);
  
  useEffect(() => {
    tryGetMatch();
  }, [props.matchID])
  useEffect(()=>{
    tryGetQuiz();
    tryGetWinner();
  },[match])

  async function tryGetMatch() {
    try {
      const response = await fetch(
        "http://localhost:5221/Match/?muID=" + props.matchID,
        {
          method: "GET",
          headers: {
            accept: "text/plain",
            SessionID: props.sessionID,
          },
        }
      );
      const json = await response.json();
      if (response.ok) {
        const data = json;
        setMatch(data)
        //if(data.getPlayerIDsScores.lenght > 0)
        const sortedScore = data.getPlayerIDsScores
        sortedScore.sort(function(a,b) {
          return b.score-a.score;
        })
        sortedScore.reverse();
        setScores(sortedScore)
      }
    } catch (error) {
      console.error(error);
    }
  }
  async function tryGetQuiz() {
    try {
      const response = await fetch("http://localhost:5221/Quiz?quID=" + match.quizID, {
        method: "GET",
        headers: {
          accept: "text/plain",
          'SessionID': localStorage.getItem('sessionID')
        },
      });
      const json = await response.json();

      if (response.ok) {
        setQuiz(json)
      }
    }
    catch (error) {
      console.error(error);
    }
  }

  async function tryGetWinner() {
    const winer = match.getPlayerIDsScores.find(l => l.account.id == match.winnerID)
    setWinerName(winer.account.username)
  }

  async function TryGetPlayers() {

  }

  return (
    <>
      <Modal open={props.open}>
        <Card sx={style} className="flex-down seperate-children-small">
          <Typography variant="h3">Match summary</Typography>
          <Typography variant="h4" color={"var(--success)"}>{winerName} Won!</Typography>
          <Typography variant="h5">Quiz: "{quiz.name}"</Typography>
          <Typography>&nbsp;</Typography>
          <Typography variant="h5" width={"80%"} textAlign={"left"}>Scoreboard:</Typography>
          <Card variant="outlined" className="padding" sx={{ width: "80%", backgroundColor: "transparent" }}>
            {
              scores.map((playerScore, index) => {
                return (<Typography key={index} variant="h6">{playerScore.account.username} - {playerScore.gameScore}</Typography>)
              })
            }
          </Card>
          <Typography width={"80%"} textAlign={"right"}>played on {match.created}</Typography>
          <Button variant="contained" size="large" color="error" onClick={props.onClick}>Close</Button>
        </Card>
      </Modal>
    </>
  );
}

export default MatchModal;
