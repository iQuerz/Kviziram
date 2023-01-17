import { Typography } from "@mui/material";

function FriendsItem(props){
    return (
        <div className="friend-card">
            <Typography variant="h6">
                {props.name}
            </Typography>
            <div className="flex-list-row">
                {
                    props.status == "online" &&
                    <div className="status-dot online"></div>
                }
                {
                    props.status == "offline" &&
                    <div className="status-dot offline"></div>
                }
                <Typography> {props.status} </Typography>
            </div>
        </div>
    )
}
export default FriendsItem