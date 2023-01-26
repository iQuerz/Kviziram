import { Typography } from "@mui/material";
import FriendsProfileModul from "./FriendProfileModul";
import { useState } from "react";

function FriendsItem(props){
    const [isFriendProfileModalOpen, setIsFriendProfileModalOpen] = useState(false);

    function handleIsFriendProfileModalOpen() {
        setIsFriendProfileModalOpen(!isFriendProfileModalOpen);
      }
    return (
        <>
        <div onClick={setIsFriendProfileModalOpen} className="friend-card">
            <img className="avatar-small" src={props.avatar}></img>
            <Typography variant="h6">
                {props.name}
            </Typography>
            <div className="flex-list-row">
                {
                    props.status == 0 &&
                    <div className="status-dot online"></div>
                }
                {
                    props.status == 1 &&
                    <div className="status-dot offline"></div>
                }
            </div>
        </div>
        <FriendsProfileModul sessionID={props.sessionID} open={isFriendProfileModalOpen} account={props} onChange={handleIsFriendProfileModalOpen}>
            
        </FriendsProfileModul>
        </>
    )
}
export default FriendsItem