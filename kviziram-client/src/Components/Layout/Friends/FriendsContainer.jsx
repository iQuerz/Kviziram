import { Card } from "@mui/material";
import { useState } from "react";
import FriendsItem from "./FriendsItem";

function FriendsContainer(){
    const [numOfFreinds, setnumOfFreinds] = useState(0);
    const [friends, setFriends] = useState([
                                                { name: "Nikola", status: "Active" },
                                                { name: "Piksi", status: "Inactive" },
                                                { name: "Lajron", status: "Active" },
                                                { name: "Neko", status: "Active" }
                                            ]);


    function handleFriends(friends){
        setFriends(friends)
    }

    return (
        <Card>
            {friends.map((friend, i) => (
                <FriendsItem key={i} name={friend.name} status={friend.status} />
            ))}
        </Card>
    )
}
export default FriendsContainer