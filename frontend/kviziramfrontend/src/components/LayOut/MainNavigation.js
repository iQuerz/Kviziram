import { Link } from "react-router-dom";

import classes from  './MainNavigation.module.css'

function MainNavigation() {
  return (
    <header className={classes.header}>
        <div className={classes.div}> Kviziram </div>
      <nav className={classes.nav}>
        <ul className={classes.ul}>
          <li>
            <Link to="/">Home</Link>
          </li>
          <li>
            <Link to="/Play-Page">Play</Link>
          </li>
          <li>
            <Link to="/Create-Page">Create</Link>
          </li>
          <li>
            <Link to="/Profile-Page">Profile</Link>
          </li>
          <li>
            <Link to="/logout">Logout</Link>
          </li>
        </ul>
      </nav>
    </header>
  );
}

export default MainNavigation;
