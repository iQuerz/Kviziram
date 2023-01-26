import { Box } from "@mui/material";
import Sidebar from "./Sidebar";

function SidebarLayout(props) {
    return(
        <>
            <Sidebar sessionID={props.sessionID}></Sidebar>
            <Box className="page-container flex-down padding">
                {props.children}
            </Box>
        </>
    )
}

export default SidebarLayout