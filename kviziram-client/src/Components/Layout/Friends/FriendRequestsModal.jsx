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

function FriendRequestModal(props) {
    const [searchString, setSearchString] = useState("");
    const [pendingReuqest, setPendingReuqest] = useState([]);

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
                    <Box width={"90%"}>
                        <Card variant="outlined" sx={{ backgroundColor: "var(--white)", border: 1 }} >
                            <Box className="flex-right wrap margin seperate-children-small" width="90%">
                            {props.requests.map((friend, i) => (
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

export default FriendRequestModal;
