import { Card, Typography } from "@mui/material"

function LobbyParticipant(props) {
    return(
        <Card className="flex-list-row width-100 padding">
            <img className="avatar-small" src={props.player.avatar}> </img>
            <Typography fontWeight='bold'>{props.player.username}</Typography>
        </Card>
    )
}

export default LobbyParticipant