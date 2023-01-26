import {
    Box,
    Button,
    Card,
    CardHeader,
    MenuItem,
    Modal,
    Select,
    TextField,
    Typography,
} from "@mui/material";

import SearchIcon from '@mui/icons-material/Search';
import { useState } from "react";

const style = {
    position: "absolute",
    top: "50%",
    left: "50%",
    transform: "translate(-50%, -50%)",
    width: "50%",
    bgcolor: "var(--white)",
    boxShadow: 24,
    p: 4,
};

function AddFriendModal(props) {
    const [searchString, setSearchString] = useState("");
    const [searchedFriends, setSearchedFriends] = useState([]);
    function updateSearchedFriends(){
        let newSearchedFriends = []
        //todo: piksi fetchuj searched friends :3 koristi searchString state promenljivu
        setSearchedFriends(newSearchedFriends);
    }
    function updateSearchString(event){
        setSearchString(event.target.value);
    }

    const [recommendedFriends, setRecommendedFriends] = useState([]);
    function updateRecommendedFriends(){
        let newRecommendedFriends = []
        //todo: piksi fetchuj recommended friends :3
        setRecommendedFriends(newRecommendedFriends);
    }

    function cancelButton() {
        props.onChange();
    }

    return (
        <>
            <Modal open={props.open}>
                <Card sx={style} className="flex-down seperate-children-small">
                    <Box className="flex-right seperate-children-small" alignItems={"center"}>
                        <Typography variant="h4">Search</Typography>
                        <TextField onChange={updateSearchString}></TextField>
                        <Button variant="contained"><SearchIcon/></Button>
                    </Box>
                    <Box width={"90%"}>
                        <Card variant="outlined" sx={{ backgroundColor: "var(--white)", border: 1 }} >
                            <Box className="flex-right wrap margin seperate-children-small" width="90%">
                                {
                                    searchedFriends.map(friend => {
                                        return (<Button>update prijatelj style</Button>);
                                    })
                                }
                            </Box>
                        </Card>
                    </Box>

                    <Typography variant="h4">Recommended Friends</Typography>
                    <Box width={"90%"}>
                        <Card variant="outlined" sx={{ backgroundColor: "var(--white)", border: 1 }} >
                            <Box className="flex-right wrap margin seperate-children-small" width="90%">
                                {
                                    recommendedFriends.map(friend => {
                                        return (<Button>update prijatelj style</Button>);
                                    })
                                }
                            </Box>
                        </Card>
                    </Box>

                    <Box className="seperate-children-medium">
                        <Button
                            variant="contained"
                            size="large"
                            color="error"
                            onClick={cancelButton}
                        >
                            Close
                        </Button>
                    </Box>
                </Card>
            </Modal>
        </>
    );
}

export default AddFriendModal;
