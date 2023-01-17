import { Link } from "react-router-dom";
import FriendsContainer from "../Friends/FriendsContainer";

function Sidebar() {
    return(
        <div className="sidebar">
            <Link to="/Play" className="sidebar-nav-button"><h2>Play</h2></Link>
            <Link to="/Create" className="sidebar-nav-button"><h2>Create</h2></Link>
            <Link to="/Profile" className="sidebar-nav-button"><h2>Profile</h2></Link>
            <Link to="/" className="sidebar-nav-button"><h2>Logout</h2></Link>
            <FriendsContainer></FriendsContainer>
        </div>
    )
}

export default Sidebar