import { Button } from "@mui/material";
import { Link } from "react-router-dom";
import FriendsContainer from "../Friends/FriendsContainer";
import Logo from "./logo";

function Sidebar(props) {

    function handleLogout(){
        tryUserLogout();
    }
        
    async function tryUserLogout(){
        try {
            const response = await fetch('http://localhost:5221/Login/logout', {
                method: 'GET',
                headers: {
                    'accept' : 'text/plain',
                    SessionID : props.sessionID
                }
            });
            const json = await response.json();
            
            if(response.ok)
            { 
                console.log(json)
            }            
        } catch (error) {
            //setErrorMsg(error)
            console.error(error);
        }
}

    return(
        <div className="sidebar">
            <Logo url={"https://cdn.discordapp.com/attachments/1049007838669852713/1064567303292846170/Kviziram-Logo.png"}> </Logo>
            <Link to="/Play" className="sidebar-nav-button"><h2>Play</h2></Link>
            <Link to="/Create" className="sidebar-nav-button"><h2>Create</h2></Link>
            <Link to="/Profile" className="sidebar-nav-button"><h2>Profile</h2></Link>
            <Link onClick={handleLogout} to="/" className="sidebar-nav-button"><h2>Logout</h2></Link>
            <FriendsContainer sessionID={props.sessionID}></FriendsContainer>
        </div>
    )
}

export default Sidebar