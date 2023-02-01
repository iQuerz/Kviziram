
import { Box, Button, Typography } from "@mui/material";
import { useState } from "react";
import { useEffect } from "react";
import MatchModal from "./MatchModal";


function MatchIcon(props) {
    const [quiz, setQuiz] = useState("");
    const [modalOpen, setModalOpen] = useState(false);

    function handleOpenModal(){
        setModalOpen(true);
    }
    function handleCloseModal(){
        setModalOpen(false);
    }

    useEffect(()=>
    {
        tryGetQuiz();
    },[])

    async function tryGetQuiz(){
        try {
            const response = await fetch("http://localhost:5221/Quiz?quID="+props.match.quizID, {
                method: "GET",
                headers: {
                    accept: "text/plain",
                    'SessionID':  localStorage.getItem('sessionID')
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

    return(
        //ovaj div je ovde da bih mogao da stackujem margin lmao
        <div>
        <Button className="padding" variant="outlined" onClick={handleOpenModal}>
            <Typography variant="h6">{quiz.name} - </Typography>
            <Typography>&nbsp; {props.match.created.toString().substring(0,10)}</Typography>
        </Button>
        <MatchModal open={modalOpen}
                    onClick={function(){
                      handleCloseModal();
                    }}
                    matchID={props.match.id}
                    sessionID={localStorage.getItem('sessionID')}>
        </MatchModal>
        </div>
    )
}

export default MatchIcon