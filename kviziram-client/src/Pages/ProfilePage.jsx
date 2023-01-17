
import { Typography } from "@mui/material";

import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";

function ProfilePage(props) {

    return(
        <>
            <SidebarLayout>
                <Typography variant="h1">{props.mySessionID}</Typography>
            </SidebarLayout>
        </>
    )
}

export default ProfilePage