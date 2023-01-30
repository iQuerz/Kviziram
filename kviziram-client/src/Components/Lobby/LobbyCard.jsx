import { Button, Card, Typography } from "@mui/material"

function LobbyCard(props) {
    return(
        <Card className="flex-list-row width-100 padding" onClick={props.handleClick}>
            <Typography fontWeight='bold'> {"Quizz : "} {props.lobby.quizName }</Typography>
            <Typography fontWeight='bold'> {"Host : "} {props.lobby.hostName} </Typography>
            <Typography fontWeight='bold'> {"Category : "} {props.lobby.categoryName} </Typography>
            <Typography fontWeight='bold'> {"Invite Code : "} {props.lobby.inviteCode} </Typography>
        </Card>
    )
}

export default LobbyCard