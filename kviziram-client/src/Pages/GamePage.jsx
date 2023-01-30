
import { Box, Card, Typography } from "@mui/material";

import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";

function GamePage(props) {

    return(
        <>
            <Box className="game-container">
                <Box className="game-left">

                    <Box className="player-info game-padding">
                        <Card className="game-card">
                            Player info ovde
                        </Card>
                    </Box>
                    <Box className="game-chat game-padding">
                        <Card className="game-card">
                            Chat ovde
                        </Card>
                    </Box>
                    
                </Box>
                <Box className="game-right">

                    <Box className="game-header game-padding">
                        <Card className="game-card">
                            Header ovde
                        </Card>
                    </Box>
                    <Box className="game-quiz game-padding">
                        <Card className="game-card">
                            Quiz ovde
                        </Card>
                    </Box>

                </Box>
            </Box>
        </>
    )
}

export default GamePage