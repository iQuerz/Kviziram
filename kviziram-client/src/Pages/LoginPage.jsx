import LoginForm from "../Components/Login/LoginForm";

import { Typography, Box, Button } from "@mui/material";
import { Link, useNavigate } from "react-router-dom";


function LoginPage() {
    const navigate = useNavigate();
    
    function tryUserLogin(){
        navigate('Play')//fali login
    }
    function tryGuestLogin(){
        navigate('Play')//da se zameni sa lobby
    }
    
    return(
        <Box className="flex-down" paddingTop={'5em'}>
            <Typography variant="h1" color="textPrimary" textAlign={'center'}>Kviziram</Typography>
            <Typography variant="h4" color="textSecondary" textAlign={'center'} maxWidth={'20em'} marginBottom={'2em'}>
                Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas pharetra bibendum tempor.
            </Typography>
            <div className="flex-right seperate-children-big">
                <LoginForm title="User Login" field1="Email" field2="Password"
                           field2Type="password" clickHandler={tryUserLogin}></LoginForm>
                <LoginForm title="Guest Login" field1="Username" field2="Quiz Code"
                           field2Type="text" clickHandler={tryGuestLogin}></LoginForm>
            </div>
            <Typography variant="subtitle1" color="textPrimary">
                Don't have an account? <Link to="Register">Sign up!</Link>
            </Typography>
        </Box>
    )
}

export default LoginPage