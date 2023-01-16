import { Typography } from "@mui/material";

function FriendsItem(props){

    return (
        <Typography>
            {props.name} Status : {props.status}
        </Typography>
    )
}
export default FriendsItem