import { Link } from "react-router-dom";

function Sidebar() {
    return(
        <div className="sidebar">
            <Link to="/Play"><h2>Play</h2></Link>
            <Link to="/Create"><h2>Create</h2></Link>
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

export default Sidebar