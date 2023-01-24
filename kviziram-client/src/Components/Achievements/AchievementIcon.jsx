
import { Box, Typography } from "@mui/material";

function AchievementIcon(props) {
    console.log("heh")
    return(
        //ovaj div je ovde da bih mogao da stackujem margin lmao
        <div>
        <Box className="flex-list-row margin">
            <img className="avatar" src={props.achievement.Picture}></img>
            <Typography variant="h6">"{props.achievement.Name}"</Typography>
        </Box>
        </div>
    )
}

export default AchievementIcon