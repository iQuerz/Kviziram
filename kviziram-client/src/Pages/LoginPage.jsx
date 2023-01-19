import LoginForm from "../Components/Login/LoginForm";

import { Typography, Box, Button } from "@mui/material";
import { Link, useNavigate } from "react-router-dom";
import { useState } from "react";


function LoginPage() {
    const navigate = useNavigate();
    const [userName, setUserName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setpassword] = useState("");
    const [code, setcode] = useState("");

    function handleUserNameChange(event){
        setUserName(event.target.value)
        console.log(userName)
    }
    function handleEmailChange(event){
        setEmail(event.target.value)
        console.log(email)
    }
    function handlePasswordChange(event){
        setpassword(event.target.value)
        console.log(password)
    }
    function handleCodeChange(event){
        setcode(event.target.value)
        console.log(code)
    }
    
    
    async function tryUserLogin(){
            try {
                const response = await fetch('http://localhost:5221/Login', {
                    method: 'GET',
                    headers: {
                        'accept' : 'text/plain',
                        'Authentication': email + ":" + password
                    }
                });
                const json = await response.text();
                const sessionId = json.substring(json.indexOf("sessionID:") + 10);
                console.log(sessionId);
                //console.log(json)
                if(response.ok)
                    handleLoginSuccess()
            } catch (error) {
                console.error(error);
            }
    }
    function handleLoginSuccess(){
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
                           field2Type="password" clickHandler={tryUserLogin} value1={email} 
                           value2={password} onChangeField1={handleEmailChange}
                           onChangeField2={handlePasswordChange}></LoginForm>
                <LoginForm title="Guest Login" field1="Username" field2="Code"
                           field2Type="text" clickHandler={tryGuestLogin} value1={userName} 
                           value2={code} onChangeField1={handleUserNameChange}
                           onChangeField2={handleCodeChange}></LoginForm>
            </div>
            <Typography variant="subtitle1" color="textPrimary">
                Don't have an account? <Link to="Register">Sign up!</Link>
            </Typography>
        </Box>
    )
}

export default LoginPage