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
  useEffect(() => {
    tryGetMatch();
  }, [props.matchID])

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
      }
    } catch (error) {
      console.error(error);
    }
  }

  async function tryGetQuiz() {
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
        console.log("match response ok")
        const data = json;
        setMatch(data)
        console.log("match date string = ", match.created)
      }
    } catch (error) {
      console.error(error);
    }
  }

  async function tryGetWinner() {

  }

  async function TryGetPlayers() {

  }

  return (
    <>
      <Modal open={props.open}>
        <Card sx={style} className="flex-down seperate-children-small">
          <Typography variant="h3">Match summary</Typography>
          <Typography variant="h4" color={"var(--success)"}>{match.winnerID} Won!</Typography>
          <Typography variant="h5">Quiz: "{match.quizID}"</Typography>
          <Typography>&nbsp;</Typography>
          <Typography variant="h5" width={"80%"} textAlign={"left"}>Scoreboard:</Typography>
          <Card variant="outlined" className="padding" sx={{ width: "80%", backgroundColor: "transparent" }}>
            {/*ovde moze loop kroz ljude u match. treba da se fetchuju*/}
            <Typography variant="h6">Piksi - 10p</Typography>
            <Typography variant="h6">iQuerz - 8p</Typography>
            <Typography variant="h6">Lajron - 8p</Typography>
          </Card>
          <Typography width={"80%"} textAlign={"right"}>played on {match.created}</Typography>
          <Button variant="contained" size="large" color="error" onClick={props.onClick}>Close</Button>
        </Card>
      </Modal>
    </>
  );
}

export default MatchModal;
