
import { Typography } from "@mui/material";
import { Link } from "react-router-dom";

function SideBar() {
    return(
        <div className="sidebar">
            <Link to="/Home"><h2>Home</h2></Link>
            <Link to="/Profile"><h2>Profile</h2></Link>
            <Link to="/"><h2>Logout</h2></Link>
            <div>
                <h2>friend 1</h2>
                <p>status 1</p>
            </div>
            <div>
                <h2>friend 2</h2>
                <p>status 2</p>
            </div>
            <div>
                <h2>friend 3</h2>
                <p>status 3</p>
            </div>
        </div>
    )
}

export default SideBar