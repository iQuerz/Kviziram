
import { Box, Button, Card, TextField, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";

import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";
import Ad from "../Components/Layout/Ad";
import PickQuizModal from "../Components/Lobby/PickQuizModal";

import { useState, useEffect } from "react";
import { useRef } from "react";
import LobbyCard from "../Components/Lobby/LobbyCard";

function PlayPage(props) {
    useEffect(() => {
        window.localStorage.setItem('sessionID', props.mySessionID.toString())
        tryGetPublicLobbies();
    }, []);
    function RefreshPublicLobbies()
    {
        console.log("refresh...")
        tryGetPublicLobbies();
    }
    const navigate = useNavigate();
    //const [lobbyCode, setLobbyCode] = useState("");
    const lobbyCode = useRef("");
    const [newLobbyName, setNewLobbyName] = useState("");
    const [isCreateLobbyModalOpen, setCreateLobbyModalOpen] = useState(false);
    const [publicLobbies, setPublicLobbies] = useState([]);
    const [selectedLobby, setSelectedLobby] = useState("");
    
    function joinLobby(){
        console.log(lobbyCode.current.value);
        window.localStorage.setItem('inviteCode', lobbyCode.current.value)
        navigate('../Lobby')
    }
    //mora da bude dve funkcije kako ne bi doslo do bugs
    function joinPublicLobby(){
        console.log(selectedLobby.inviteCode);
        window.localStorage.setItem('inviteCode', selectedLobby.inviteCode)
        navigate('../Lobby')
    }

    function newLobbyNameInputChanged(event){
        setNewLobbyName(event.target.value);
    }
    function handleCreateLobbyModalOpenChange(){
        setCreateLobbyModalOpen(!isCreateLobbyModalOpen);
    }
    function handleSelectedLobbyChange(event)
    {
        setSelectedLobby(event)
    }
    async function tryGetPublicLobbies()
    {   
        try {
          const endpoint = "http://localhost:5221/Game/public/0/20/false";
          const body = JSON.stringify({
            fromDate : "2022-01-30T10:36:05.954Z",
            toDate : "2024-01-30T10:36:05.954Z"
          });
          const response = await fetch(endpoint, {
            method: "POST",
            body: body,
            headers: {  "accept": "text/plain", 
                        "Content-Type": "application/json",}
          });
          const json = await response.json();
          if (!response.ok) {
            throw json;
          }
          if(json)
          {
            //console.log(json);
            //setInviteCode(json.inviteCode)
            setPublicLobbies(json)
          }
        } catch (error) {
          throw error;
        }     
    }  

    return(
        <>
            <SidebarLayout  sessionID={props.mySessionID}>
                <Typography variant="h1" color="textPrimary">Play!</Typography>
                <Typography variant="h4" color="textSecondary" textAlign="center" maxWidth={'20em'} marginBottom={'2em'}>
                    Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas pharetra bibendum tempor.
                </Typography>

                <Card className="public-lobbies-card width-75 flex-down seperate-children-small padding">
                    <Button variant="contained" size="large" onClick={RefreshPublicLobbies}>Refresh</Button>
                    <Typography variant="h4">Browse Public Lobbies</Typography>
                    {/* <Typography variant="h4">Selected lobby : {selectedLobby.inviteCode}</Typography> */}
                    <Card className="flex-right wrap" sx={{overflow: "scroll"}}>
                        {
                        publicLobbies.map((lobby, index) => {
                                return (<LobbyCard  handleClick={()=>{
                                    handleSelectedLobbyChange(lobby);
                                }} 
                                lobby={lobby} key={index} onClick={joinPublicLobby}></LobbyCard>)
                            })
                    }
                    </Card>
                    {/* <Button variant="contained" size="large" onClick={joinPublicLobby}>Join</Button> */}
                </Card>

                <Ad></Ad>

                <Box className="flex-right width-75 seperate-children-big">
                    <Card className="width-35 flex-down seperate-children-small padding">
                        <Typography variant="h4">Join via Code</Typography>
                        <TextField label="Lobby code" inputRef={lobbyCode}></TextField>
                        <Button variant="contained" size="large" onClick={joinLobby}>Join</Button>
                    </Card>
                    <Card className="width-35 flex-down seperate-children-small padding">
                        <Typography textAlign={"center"} variant="h4">Host a new match</Typography>
                        
                        {/* <TextField label="Lobby name" onChange={newLobbyNameInputChanged}></TextField> */}
                        <Button variant="contained" size="large" onClick={handleCreateLobbyModalOpenChange}>Host</Button>
                    </Card>
                </Box>

                <Ad></Ad>
            
                <PickQuizModal sessionID={props.mySessionID} open={isCreateLobbyModalOpen} onChange={handleCreateLobbyModalOpenChange} name={newLobbyName}></PickQuizModal>
            </SidebarLayout>
        </>
    )
}

export default PlayPage