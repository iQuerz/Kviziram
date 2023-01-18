import { Card, Typography } from "@mui/material"

function LobbyParticipant(props) {
    return(
        <Card className="flex-list-row width-100 padding">
            <Typography fontWeight='bold'>{props.player.username}</Typography>
        </Card>
    )
}

export default LobbyParticipant