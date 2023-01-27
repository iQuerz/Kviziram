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
            <img className="avatar-small" src={props.account.avatar}></img>
            <Typography variant="h6">
                {props.account.username}
            </Typography>
            <div className="flex-list-row">
                {
                    props.account.status == 0 &&
                    <div className="status-dot online"></div>
                }
                {
                    props.account.status == 1 &&
                    <div className="status-dot offline"></div>
                }
            </div>
        </div>
        <FriendsProfileModul sessionID={props.sessionID} open={isFriendProfileModalOpen} account={props.account} onChange={handleIsFriendProfileModalOpen}>
            
        </FriendsProfileModul>
        </>
    )
}
export default FriendsItem