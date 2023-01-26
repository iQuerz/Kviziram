
import { Typography } from "@mui/material";

import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";

function GamePage(props) {

    return(
        <>
            <SidebarLayout sessionID={props.mySessionID}>
                <Typography variant="h1">Not yet implemented</Typography>
            </SidebarLayout>
        </>
    )
}

export default GamePage