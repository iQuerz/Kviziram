import { Box, Button, Card, Divider, Typography } from "@mui/material";
import { useState } from "react";
import AddFriendModal from "./AddFriendModal";
import FriendsItem from "./FriendsItem";

function FriendsContainer(){
    const [isAddFriendModalOpen, setAddFriendModalOpen] = useState(false);
    const [numOfFreinds, setnumOfFreinds] = useState(0);
    const [friends, setFriends] = useState([
                                                { name: "Nikola", status: "online" },
                                                { name: "Piksi", status: "offline" },
                                                { name: "Lajron", status: "online" },
                                                { name: "Neko", status: "online" }
                                            ]);


    function handleFriends(friends){
        setFriends(friends)
    }

    function handleAddFriendsModalOpenChange(){
        setAddFriendModalOpen(!isAddFriendModalOpen);
    }

    return (
        <>
            <Divider variant="middle" className="sidebar-divider"></Divider>
            <Box className="friends-container">
                <Typography variant="h4" fontWeight={"bold"}>
                    My friends <Button variant="contained" fullWidth="true" onClick={handleAddFriendsModalOpenChange}>+
                               </Button>
                </Typography>
                
                {friends.map((friend, i) => (
                    <FriendsItem key={i} name={friend.name} status={friend.status} />
                ))}
            </Box>
            <AddFriendModal open={isAddFriendModalOpen} onChange={handleAddFriendsModalOpenChange}></AddFriendModal>
        </>
    )
}
export default FriendsContainer