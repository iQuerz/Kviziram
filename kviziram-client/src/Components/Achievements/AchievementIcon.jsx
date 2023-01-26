
import { Box, Typography } from "@mui/material";

function AchievementIcon(props) {
    return(
        //ovaj div je ovde da bih mogao da stackujem margin lmao
        <div>
        <Box className="flex-list-row margin">
            <img className="avatar" src={props.achievement.picture}></img>
            <Typography variant="h6">"{props.achievement.name}" x10</Typography>
        </Box>
        </div>
    )
}

export default AchievementIcon