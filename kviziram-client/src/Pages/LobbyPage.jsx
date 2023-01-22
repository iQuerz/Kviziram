
import { Box, Button, Card, List, Typography } from "@mui/material";
import { useEffect } from "react";
import { useState } from "react";
import ChatContainer from "../Components/Chat/ChatContainer";

import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";
import LobbyParticipant from "../Components/Lobby/LobbyParticipant";

import * as signalR from "@microsoft/signalr"
import AppData from "../js/AppData";

function LobbyPage(props) {
    AppData.loadTestData();
    let [players, setPlayers] = useState([
        {
            username: "iQuerz"
        },
        {
            username: "Piksi"
        },
        {
            username: "Lajron"
        }
    ]);
    function updatePlayers(){
        //ovde bi isao fetch za playere iz lobby, ovu funkciju bi pozvao signalR as well
        let array = [
            {
                username: "iQuerz"
            },
            {
                username: "Piksi"
            },
            {
                username: "Lajron"
            }
        ];
        setPlayers(array);
    }
    var hubConnection;
    useEffect(() => {
        hubConnection = new signalR.HubConnectionBuilder().withUrl("/hubs/game").build();
        hubConnection.start().then(hubConnectionSuccess, hubConnectionFailure);

        hubConnection.on("connect", function () {
            //todo: reload podatke
        })
        hubConnection.on("receiveGameStarted", function(){
            //todo: prebaci se na game page
        })
        
        startGame = function(){
            hubConnection.send("StartGame", AppData.CurrentMatch.inviteCode.toString());
        }
    })
    function hubConnectionSuccess() {
        hubConnection.send("OnJoinGame", AppData.CurrentMatch.inviteCode.toString());
    }
    function hubConnectionFailure() {
        alert("hub connection unsuccesful.");
    }

    let startGame = function(){}
    

    return (
        <>
            <SidebarLayout>
                <Typography variant="h1" color="textPrimary" marginBottom="0.5em">Dzoni's Lobby</Typography>

                <Card className="flex-right seperate-children-small padding">
                    <Typography variant="h5">Selected Quiz:</Typography>
                    <Typography variant="h5" fontWeight="bold">"props.quiz.name"</Typography>
                    <Typography variant="h5">[props.quiz.category]</Typography>
                </Card>

                <Box className="flex-right seperate-children-big">
                    <Button variant="contained" size="large" onClick={startGame()} color="success">Start</Button>
                    <Button variant="contained" size="large">Invite</Button>
                    <Button variant="contained" size="large" color="error">Leave</Button>
                </Box>

                <Box marginTop={"1.5em"} className="width-50 flex-down seperate-children-small">
                    <Typography variant="h5">Players:</Typography>
                    {
                        players.map(player => {
                            return (<LobbyParticipant player={player}></LobbyParticipant>)
                        })
                    }
                </Box>

                <ChatContainer></ChatContainer>

            </SidebarLayout>
        </>
    )
}

export default LobbyPage