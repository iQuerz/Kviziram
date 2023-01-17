
import { Typography } from "@mui/material";
import { useState } from "react";

import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";

function LobbyPage() {
    const [players, setPlayers] = useState();

    return(
        <>
            <SidebarLayout>
                <Typography variant="h1">Not yet implemented</Typography>
            </SidebarLayout>
        </>
    )
}

export default LobbyPage