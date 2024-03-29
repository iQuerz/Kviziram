
import { Box, Button, Card, List, Typography } from "@mui/material";
import { useEffect } from "react";
import { useState, useRef,useMemo } from "react";
import ChatContainer from "../Components/Chat/ChatContainer";

import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";
import LobbyParticipant from "../Components/Lobby/LobbyParticipant";

import * as signalR from "@microsoft/signalr"
import AppData from "../js/AppData";
import { useNavigate } from "react-router-dom";

var hubConnection;

function LobbyPage(props) {
    AppData.loadTestData();
    const [players, setPlayers] = useState([]);
    //const [inviteCode, setInviteCode] = useState();
    const [quizzName, setQuizzName] = useState("");
    const [quizzCategory, setQuizzCategory] = useState("");
    const [msgRecived, setMsgRecived] = useState(0);
    const [inviteCodeState, setinviteCodeState] = useState("");
    const inviteCode = useRef("");
    const navigate = useNavigate();
    
    //var hubConnection;

    useEffect(() => {
        const invCode = window.localStorage.getItem('inviteCode');
        inviteCode.current = invCode;
        setinviteCodeState(invCode)
        hubConnection = new signalR.HubConnectionBuilder().withUrl("ws://localhost:5221/hubs/game?sId=" + props.mySessionID,{
            skipNegotiation : true,
            transport : signalR.HttpTransportType.WebSockets
        })
        .withAutomaticReconnect()
        .build();
        hubConnection.start().then(hubConnectionSuccess, hubConnectionFailure);

        hubConnection.on("receiveConnected", function () { 
            //console.log("Neko se connectovao u game")
            tryGetLobbyMembers();
        })
        hubConnection.on("receiveDisconnected", function () { 
          //console.log("Neko se disconectovao iz gama")
          tryGetLobbyMembers();
      })
      hubConnection.on("receiveChatMessage", function () { 
        console.log("stigla mi prouka..")
        handleMsgRecived();
      })
    
         hubConnection.on("receiveGameStarted", function(){
          console.log("Game started")
          
          navigate('/Game')
        })

        props.onHubChange(hubConnection);

        tryGetLobbyMembers();
        tryGetLobbyInformation();
        tryGetLobby();
    },[])
    function print(){
      console.log(hubConnection);
    }
    function handleMsgRecived(){
      setMsgRecived((msgRecived) => msgRecived + 1)
    }
    function SendMsg(msg) {
      hubConnection.send("SendChatMessage", msg)
    }
    async function handleStartGame () {
      tryStartGame();
    };

    function hubConnectionSuccess() {
        //console.log("hvala kurcu ovo radi")
        //alert("hvala kurcu ovo radi");
        hubConnection.send("OnJoinGame", inviteCode.current);
    }

    function hubConnectionFailure() {
        console.log("hub connection unsuccesful.")
        alert("hub connection unsuccesful.");
    }

    function leave(){
      hubConnection.stop();
      navigate("/Play")
    }

     async function tryGetLobbyMembers() {
        try {
          const response = await fetch(
            "http://localhost:5221/Game/" + inviteCode.current + "/lobby",
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
            //console.log(data)          
            setPlayers(data);
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
            //console.log(json);
            setQuizzName(json.quizName)
            setQuizzCategory(json.categoryName)
            //setPlayers(json);
          }
        } catch (error) {
          console.error(error);
        }
      }

      //sta got to znacilo 
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
            //console.log(json);
            //setPlayers(json);
          }
        } catch (error) {
          console.error(error);
        }
      }
      async function tryStartGame() {
        try {
          const response = await fetch(
            "http://localhost:5221/Game/" + inviteCode.current + "/start",
            {
              method: "GET",
              headers: {
                accept: "text/plain",
                SessionID: props.mySessionID,
              },
            }
          );
          //const json = await response.json();
    
          if (response.ok) {
            //console.log(json)
          }
        } catch (error) {
          console.error(error);
        }
      }

    return (
        <>
            <SidebarLayout  sessionID={props.mySessionID}>
                <Typography variant="h1" color="textPrimary" marginBottom="0.5em">Lobby : {inviteCode.current}</Typography>

                <Card className="flex-right seperate-children-small padding">
                    <Typography variant="h5">Selected Quiz:</Typography>
                    <Typography variant="h5" fontWeight="bold">"{quizzName}"</Typography>
                    <Typography variant="h5">({quizzCategory})</Typography>
                </Card>

                <Box className="flex-right seperate-children-big">
                    <Button variant="contained" size="large" onClick={handleStartGame} color="success">Start</Button>
                    <Button variant="contained" size="large" onClick={print}>Check Connetion</Button>
                    <Button variant="contained" size="large" onClick={leave}color="error">Leave</Button>
                </Box>

                <Box marginTop={"1.5em"} className="width-50 flex-down seperate-children-small">
                    <Typography variant="h5">Players:</Typography>
                    {
                        players.map((player, index) => {
                            return (<LobbyParticipant  player={player} key={index} ></LobbyParticipant>)
                        })
                    }
                </Box>

                <Card sx={{width:"50%", height:"20%", position:"fixed", bottom:"0", marginBottom:"1em", overflow:"auto"}} className="padding">
                  <ChatContainer sx={{height:"100%", width:"100%"}} sendMsg={SendMsg} inviteCode={inviteCodeState} msgRecived={msgRecived} sessionId={props.mySessionID}></ChatContainer>
                </Card>

            </SidebarLayout>
        </>
    )
}

export default LobbyPage