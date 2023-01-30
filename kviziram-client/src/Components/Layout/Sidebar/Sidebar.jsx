import { Button } from "@mui/material";
import { Link } from "react-router-dom";
import FriendsContainer from "../Friends/FriendsContainer";
import Logo from "./logo";

function Sidebar(props) {
    return(
        <div className="sidebar">
            <Logo url={"https://cdn.discordapp.com/attachments/1049007838669852713/1064567303292846170/Kviziram-Logo.png"}> </Logo>
            <Link to="/Play" className="sidebar-nav-button"><h2>Play</h2></Link>
            <Link to="/Create" className="sidebar-nav-button"><h2>Create</h2></Link>
            <Link to="/Profile" className="sidebar-nav-button"><h2>Profile</h2></Link>
            <Link to="/" className="sidebar-nav-button"><h2>Logout</h2></Link>
            <FriendsContainer sessionID={props.sessionID}></FriendsContainer>
        </div>
    )
}

export default Sidebar