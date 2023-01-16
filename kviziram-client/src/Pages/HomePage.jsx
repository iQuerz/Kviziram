
import { Card, Typography } from "@mui/material";
import { Box } from "@mui/system";
import { Link } from "react-router-dom";
import SideBar from "../Components/Layout/SideBar";

function HomePage() {
    return(
        <>
            <SideBar></SideBar>
            <Box className="non-sidebar flex-down">
                <Card>
                    <Typography variant="h6">Public Lobbies</Typography>
                </Card>
                <Typography variant="h1">Not yet implemented</Typography>
            </Box>
        </>
    )
}
