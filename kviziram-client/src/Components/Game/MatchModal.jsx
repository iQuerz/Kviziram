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

function MatchModal(props) {
    const [match, setMatch] = useState("");
    useEffect(()=>{
        console.log(props.matchID)
        console.log(props.sessionID)
        tryGetMatch();
    },[props.matchID])

    async function tryGetMatch() {
        try {
          const response = await fetch(
            "http://localhost:5221/Match/?muID=" + props.matchID ,
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
            console.log(data)
            setMatch(data)    
          }
        } catch (error) {
          console.error(error);
        }
      }

    return (
        <>
            <Modal open={props.open}>
                <Card sx={style} className="flex-down seperate-children-small">
                    <Typography> The winner is {match.winnerID}</Typography>
                    <Typography> Resio te</Typography>
                </Card>
            </Modal>
        </>
    );
}

export default MatchModal;
