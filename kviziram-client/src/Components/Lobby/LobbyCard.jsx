import { Button, Box, Typography } from "@mui/material"
import { useNavigate } from "react-router-dom";

function LobbyCard(props) {
    const navigate = useNavigate();

    function joinLobby (){
        window.localStorage.setItem('inviteCode', props.lobby.inviteCode);
        navigate('../Lobby');
    }
    return(
        <Box sx={{ textTransform: 'none' }} className="flex-list-row width-100 padding" onClick={props.handleClick}>
            <Typography variant="h6">{"\"" + props.lobby.quizName + "\""} &nbsp;</Typography>
            <Typography variant="h6">{"by " + props.lobby.hostName} &nbsp; </Typography>
            <Typography variant="h6"> {"(" + props.lobby.categoryName + ")"} &nbsp;</Typography>
            <Button variant="contained" size="large" onClick={joinLobby}>Join</Button>
        </Box>
    )
}

export default LobbyCard