
import { TabPanelUnstyled } from "@mui/base";
import { Box, Card, Typography } from "@mui/material";
import { func } from "prop-types";
import { useEffect, useState, useRef } from "react";
import { useNavigate } from "react-router-dom";
import ChatContainer from "../Components/Chat/ChatContainer";
import CurrentQuestion from "../Components/Game/CurrentQuestion";
import MatchModal from "../Components/Game/MatchModal";

function GamePage(props) {
    const navigate = useNavigate();

    const [selectedAnswer, setSelectedAnswer] = useState(-1)
    const [msgRecived, setMsgRecived] = useState(0);
    const [quizzID, setQuizzID] = useState("");
    const quizzIDref = useRef("");
    const [inviteCodeState, setinviteCodeState] = useState("");
    const inviteCode = useRef("");
    const [quizzName, setQuizzName] = useState("");
    const [quizzCategory, setQuizzCategory] = useState("");
    const [hostName, setHostName] = useState("");
    const [matchID, setMatchID] = useState("");
    const [scores, setScores] = useState([]);
    const [isEndGameModalOpen, setIsEndGameModalOpen] = useState(false);
    const [currentQuestion, setCurrentQuestion] = useState({options : []});

    
    useEffect( () => {
        const invCode = window.localStorage.getItem('inviteCode');
        inviteCode.current = invCode;
        setinviteCodeState(invCode)

        tryGetLobby();
        tryGetLobbyInformation();
        tryGetLobbyScores();
    },[])
    useEffect(() =>{
        tryGetNextQuestion();
    },[quizzIDref.current])

    useEffect( () => { 

        console.log(props.hubConnection)
        props.hubConnection.on("receiveChatMessage", function () { 
            handleMsgRecived();
          })
        props.hubConnection.on("receiveNextQuestion", function () { 
            handleNextQuestion();
            tryGetLobbyScores();
          })
          props.hubConnection.on("receiveFinishedGame", function (matchID) { 
            setMatchID(matchID);
            handleGameFinished();
          })
    },[props.hubConnection])

    function handleGameFinished(){
        setIsEndGameModalOpen(true);
    }
    function handleMsgRecived() { 
        setMsgRecived((msgRecived) => msgRecived + 1)
    }
    function handleAnswerChange(answer) {
        setSelectedAnswer(answer)
    }
    function handleSubmitQuestion(){
        console.log("Selected answer is")
        console.log(selectedAnswer)
        console.log(selectedAnswer.toString())
        props.hubConnection.send("SendAnswer", selectedAnswer.toString())
        setSelectedAnswer(-1);
    }
    function SendMsg(msg) { 
        props.hubConnection.send("SendChatMessage", msg)
      }

      function handleNextQuestion(){
        tryGetNextQuestion();
      }

      async function tryGetNextQuestion() {
        try {
          const response = await fetch(
            "http://localhost:5221/Game/" + inviteCode.current + "/question/" + quizzIDref.current,
            {
              method: "GET",
              headers: {
                accept: "text/plain",
                SessionID: props.mySessionID,
              },
            }
          ); 
          const json = await response.json();
          if (response.ok) {
            const data = json;  
            console.log(data);
            setCurrentQuestion(data);
          }
        } catch (error) {
          console.error(error);
        }
      }
 
      async function tryGetLobby() {
        try {
          const response = await fetch(
            "http://localhost:5221/Game/" + inviteCode.current,
            {
              method: "GET",
              headers: {
                accept: "text/plain",
                SessionID: props.mySessionID,
              },
            }
          );
          const json = await response.json();
    
          if (response.ok) {
            setQuizzID(json.quizID)
            quizzIDref.current = json.quizID;
            //setPlayers(json);
          }
        } catch (error) {
          console.error(error);
        }
      }
      async function tryGetLobbyInformation() {
        try {
          const response = await fetch(
            "http://localhost:5221/Game/" + inviteCode.current + "/information",
            {
              method: "GET",
              headers: {
                accept: "text/plain",
                SessionID: props.mySessionID,
              },
            }
          );
          const json = await response.json();
    
          if (response.ok) {
            setQuizzName(json.quizName)
            setQuizzCategory(json.categoryName)
            setHostName(json.hostName)
            //setPlayers(json);
          }
        } catch (error) {
          console.error(error);
        }
      }
      async function tryGetLobbyScores() {
        try {
          const response = await fetch(
            "http://localhost:5221/Game/" + inviteCode.current + "/scores",
            {
              method: "GET",
              headers: {
                accept: "text/plain",
                SessionID: props.mySessionID,
              },
            }
          );
          const json = await response.json();
    
          if (response.ok) {
            console.log("Game Score is :");
            console.log(json);
            setScores(json);
          }
        } catch (error) {
          console.error(error);
        }
      }
    return(
        <>
            <Box className="game-container">
                <Box className="game-left">

                    <Box className="player-info game-padding">
                        <Card className="game-card">
                           {
                            scores.sort().map((Score, index)=>(
                                <Typography margin={"0.5em"} variant="h6" key={index} >{Score.account.username} - {Score.score} points</Typography>
                            ))
                           }
                        </Card>
                    </Box>
                    <Box className="game-chat game-padding">
                        <Card className="game-card" sx={{overflow:"auto"}}>
                            <ChatContainer sendMsg={SendMsg} inviteCode={inviteCodeState} msgRecived={msgRecived} sessionId={props.mySessionID}></ChatContainer>
                        </Card>
                    </Box>
                    
                </Box>
                <Box className="game-right">

                    <Box className="game-header game-padding">
                        <Card className="game-card flex-right" sx={{alignItems:"center"}}>
                            <Typography variant="h2">{quizzName} &nbsp;</Typography>
                            <Typography variant="h2"> {"(" + quizzCategory + ")"} &nbsp;</Typography>
                        </Card>
                    </Box>
                    <Box className="game-quiz game-padding">
                        <Card className="game-card">
                            <CurrentQuestion selectedAnswer={selectedAnswer} setAnswer={handleAnswerChange} question={currentQuestion} submitQuestion={handleSubmitQuestion}/>
                        </Card>
                    </Box>

                </Box>
            </Box>
                <MatchModal         
                    open={isEndGameModalOpen}
                    onChange={setIsEndGameModalOpen}
                    onClick={function(){
                      navigate("../Play")
                    }}
                    matchID={matchID}
                    sessionID={props.mySessionID}>
                </MatchModal>
            
        </>
    )
}

export default GamePage