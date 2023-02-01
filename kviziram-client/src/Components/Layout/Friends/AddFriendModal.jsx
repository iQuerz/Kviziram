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
import FriendsItem from "./FriendsItem";
import { useState } from "react";
import { useEffect } from "react";

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

    useEffect(() => {
      tryGetRecommendedFriends();
    },[])
    function handleSearchStringChange(event){
        setSearchString(event.target.value);
    }
    function updateSearchString(){
        tryGetFriendsFromSerach()
        console.log(searchedFriends);
    }

    const [recommendedFriends, setRecommendedFriends] = useState([]);

    async function tryGetFriendsFromSerach() {
        console.log(searchString)
        try {
          const response = await fetch("http://localhost:5221/Account/search/name="+ searchString, {
            method: "GET",
            headers: {
              accept: "text/plain",
              'SessionID': props.sessionID
            },
          });
          const json = await response.json();
    
          if (response.ok) {
            setSearchedFriends(json); //predaje se sessionID app komponenti
          }
        } catch (error) {
          console.error(error);
        }
      }
      async function tryGetRecommendedFriends() {
        try {
          const response = await fetch("http://localhost:5221/Account/me/friends/recommended", {
            method: "GET",
            headers: {
              accept: "text/plain",
              'SessionID': props.sessionID
            },
          });
          const json = await response.json();
    
          if (response.ok) {
            setRecommendedFriends(json); //predaje se sessionID app komponenti
          }
        } catch (error) {
          console.error(error);
        }
      }

    async function tryGetSendFriendRequest() {
        try {
          const response = await fetch("http://localhost:5221/Account/me/friend/911cb3c0-92f2-4dae-92cb-26e5d0167b89/request", {
            method: "GET",
            headers: {
              accept: "text/plain",
            },
          });
          const json = await response.json();
    
          if (response.ok) {
            //setCategoryOptions(json); //predaje se sessionID app komponenti
          }
        } catch (error) {
          console.error(error);
        }
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
                        <TextField onChange={handleSearchStringChange}></TextField>
                        <Button onClick={updateSearchString} variant="contained"><SearchIcon/></Button>
                    </Box>
                    <Box width={"90%"}>
                        <Card variant="outlined" sx={{ backgroundColor: "var(--white)", border: 1 }} >
                            <Box className="flex-right wrap margin seperate-children-small" width="90%">
                            {searchedFriends.map((friend, i) => (
                                <FriendsItem key={i} account={friend} sessionID={props.sessionID}/>
                                ))}
                            </Box>
                        </Card>
                    </Box>

                    <Typography variant="h4">Recommended Friends</Typography>
                    <Box width={"90%"}>
                        <Card variant="outlined" sx={{ backgroundColor: "var(--white)", border: 1 }} >
                            <Box className="flex-right wrap margin seperate-children-small" width="90%">
                             {recommendedFriends.map((friend, i) => (
                                <FriendsItem key={i} account={friend} sessionID={props.sessionID}/>
                                ))}
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
