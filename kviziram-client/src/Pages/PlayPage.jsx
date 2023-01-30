
import { Box, Button, Card, TextField, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";

import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";
import Ad from "../Components/Layout/Ad";
import PickQuizModal from "../Components/Lobby/PickQuizModal";

import { useState, useEffect } from "react";
import { useRef } from "react";

function PlayPage(props) {
    useEffect(() => {
        window.localStorage.setItem('sessionID', props.mySessionID.toString())
    }, []);

    const navigate = useNavigate();
    //const [lobbyCode, setLobbyCode] = useState("");
    const lobbyCode = useRef("");
    const [newLobbyName, setNewLobbyName] = useState("");
    const [isCreateLobbyModalOpen, setCreateLobbyModalOpen] = useState(false);
    
    function joinLobby(){
        console.log(lobbyCode.current.value);
        window.localStorage.setItem('inviteCode', lobbyCode.current.value)
        navigate('../Lobby')
    }

    function newLobbyNameInputChanged(event){
        setNewLobbyName(event.target.value);
    }
    function handleCreateLobbyModalOpenChange(){
        setCreateLobbyModalOpen(!isCreateLobbyModalOpen);
    }

    return(
        <>
            <SidebarLayout  sessionID={props.mySessionID}>
                <Typography variant="h1" color="textPrimary">Play!</Typography>
                <Typography variant="h4" color="textSecondary" textAlign="center" maxWidth={'20em'} marginBottom={'2em'}>
                    Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas pharetra bibendum tempor.
                </Typography>
                <Card className="width-75 flex-down seperate-children-small padding">
                    <Typography variant="h4">Browse Public Lobbies</Typography>
                    <div className="flex-right wrap">
                        {/*todo: public lobbies */}
                    </div>
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