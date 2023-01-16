import { Box } from "@mui/material";
import Sidebar from "./Sidebar";

function SidebarLayout({children}) {
    return(
        <>
            <Sidebar></Sidebar>
            <Box className="page-container flex-down padding">
                {children}
            </Box>
        </>
    )
}

export default SidebarLayout