import { Box, Card, Divider, Typography } from "@mui/material";
import { useState } from "react";
import FriendsItem from "./FriendsItem";

function FriendsContainer(){
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

    return (
        <>
            <Divider variant="middle" className="sidebar-divider"></Divider>
            <Box className="friends-container">
                <Typography variant="h4" fontWeight={"bold"}>My friends</Typography>
                {friends.map((friend, i) => (
                    <FriendsItem key={i} name={friend.name} status={friend.status} />
                ))}
            </Box>
        </>
    )
}
export default FriendsContainer