import { Button, Card, Typography } from "@mui/material"

function LobbyCard(props) {
    return(
        <Button className="flex-list-row width-100 padding" onClick={props.handleClick}>
            <Typography fontWeight='bold'> &nbsp; Quizz :  {props.lobby.quizName }</Typography>
            <Typography fontWeight='bold'> &nbsp; Host :  {props.lobby.hostName} </Typography>
            <Typography fontWeight='bold'> &nbsp; Category : {props.lobby.categoryName} </Typography>
            <Typography fontWeight='bold'> &nbsp; Invite Code : {props.lobby.inviteCode} </Typography>
        </Button>
    )
}

export default LobbyCard