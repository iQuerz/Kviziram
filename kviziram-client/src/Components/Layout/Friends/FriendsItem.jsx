import { Typography } from "@mui/material";

function FriendsItem(props){
    //<Typography> {props.status ? "online" : "offline"} </Typography>
    return (
        <div className="friend-card">
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
    )
}
export default FriendsItem