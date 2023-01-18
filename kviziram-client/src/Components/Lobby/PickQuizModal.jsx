
import { Box, Button, Card, CardHeader, MenuItem, Modal, Select, TextField, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const style = {
    position: 'absolute',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    width: '50%',
    bgcolor: 'var(--white)',
    boxShadow: 24,
    p: 4
};


function PickQuizModal(props) {
    const navigate = useNavigate();
    
    const [open, setOpen] = useState(props.open)

    function cancelButton(){
        setOpen(false);
    }
    function startButton(){
        //ovde fetch za pravljenje lobbyja i prosledjivanje osobe u lobby

        setOpen(false)
        navigate('/Lobby');
    }

    useEffect(() => {
        setOpen(props.open)
    }, [props.open])

    return(
        <>
            <Modal open={open}>
                <Card sx={style} className="flex-down seperate-children-small">
                    <Typography variant="h2">{props.name}</Typography>

                    <Typography variant="h4">Choose a quiz</Typography>

                    <Box width={"90%"}>
                        <Card variant="outlined" sx={{backgroundColor:"var(--purple)", border:1}}>
                            <Typography fontWeight={"bold"} p={1}>My quizzes:</Typography>
                            <Box className="flex-right wrap margin seperate-children-small">
                                <Card className="padding">
                                    <Typography>Heya! Quiz1</Typography>
                                </Card>
                            </Box>
                        </Card>
                    </Box>

                    <Box width={"90%"}>
                        <Card variant="outlined" sx={{backgroundColor:"var(--purple)", border:1}}>
                            <Box className="flex-list-row">
                                <Typography fontWeight={"bold"} p={1}>Browse:</Typography>
                                <Select label="Category">
                                    <MenuItem>Music</MenuItem>
                                    <MenuItem>Sport</MenuItem>
                                    <MenuItem>Gaming</MenuItem>
                                </Select>
                            </Box>
                            <Box className="flex-right wrap margin seperate-children-small" width="90%">
                                <Card className="padding">
                                    <Typography>Heya! Quiz1</Typography>
                                </Card>
                                <Card className="padding">
                                    <Typography>Heya! Quiz1</Typography>
                                </Card>
                                <Card className="padding">
                                    <Typography>Heya! Quiz1</Typography>
                                </Card>
                                <Card className="padding">
                                    <Typography>Heya! Quiz1</Typography>
                                </Card>
                            </Box>
                        </Card>
                    </Box>

                    <Box className="seperate-children-medium">
                        <Button variant="contained" size="large" color="success">Start</Button>
                        <Button variant="contained" size="large" color="error" onClick={cancelButton}>Cancel</Button>
                    </Box>
                    
                </Card>
            </Modal>
        </>
    )
}

export default PickQuizModal