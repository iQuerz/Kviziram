import LoginForm from "../Components/Login/LoginForm";

import { Typography, Box, Button } from "@mui/material";
import { Link, useNavigate } from "react-router-dom";


function LoginPage() {
    const navigate = useNavigate();
    return(
        <Box className="flex-down" marginTop={'5em'}>
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

    function tryUserLogin(){
        navigate('Home')//fali login
    }
    function tryGuestLogin(){
        navigate('Home')//da se zameni sa lobby
    }
}

export default LoginPage