
import { Box, Button, Card, TextField, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";

import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";
import Ad from "../Components/Layout/Ad";
import PickQuizModal from "../Components/Lobby/PickQuizModal";
import { useState } from "react";

function PlayPage(props) {
    const navigate = useNavigate();
    const [lobbyCode, setLobbyCode] = useState("");
    const [newLobbyName, setNewLobbyName] = useState("");
    const [isCreateLobbyModalOpen, setCreateLobbyModalOpen] = useState(false);
    
    function lobbyCodeInputChanged(event){
        setLobbyCode(event.target.value);
    }
    function joinLobby(){
        console.log(props.sesisonID)
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
            <SidebarLayout>
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
                        <TextField label="Lobby code" onChange={lobbyCodeInputChanged}></TextField>
                        <Button variant="contained" size="large" onClick={joinLobby}>Join</Button>
                    </Card>
                    <Card className="width-35 flex-down seperate-children-small padding">
                        <Typography textAlign={"center"} variant="h4">Host a new match</Typography>
                        
                        {/* <TextField label="Lobby name" onChange={newLobbyNameInputChanged}></TextField> */}
                        <Button variant="contained" size="large" onClick={handleCreateLobbyModalOpenChange}>Host</Button>
                    </Card>
                </Box>
                <Ad></Ad>
            
                <PickQuizModal open={isCreateLobbyModalOpen} onChange={handleCreateLobbyModalOpenChange} name={newLobbyName}></PickQuizModal>
            </SidebarLayout>
        </>
    )
}

export default PlayPage