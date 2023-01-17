
import { Box, Button, Card, TextField, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";

import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";
import Ad from "../Components/Layout/Ad";

function PlayPage(props) {
    const navigate = useNavigate();
    
    function joinLobby(){
        console.log(props.sesisonID)
        navigate('../Lobby')
    }
    function createLobby(){
        navigate('../Lobby')
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
                        {/* public lobbies */}
                    </div>
                </Card>
                <Ad></Ad>
                <Box className="flex-right width-75 seperate-children-big">
                    <Card className="width-35 flex-down seperate-children-small padding">
                        <Typography variant="h4">Join via Code</Typography>
                        <TextField label="Lobby code"></TextField>
                        <Button variant="contained" size="large" onClick={joinLobby}>Join</Button>
                    </Card>
                    <Card className="width-35 flex-down seperate-children-small padding">
                        <Typography variant="h4">Create Lobby</Typography>
                        <TextField label="Lobby name"></TextField>
                        <Button variant="contained" size="large" onClick={createLobby}>Create</Button>
                    </Card>
                </Box>
                <Ad></Ad>
            </SidebarLayout>
        </>
    )
}

export default PlayPage