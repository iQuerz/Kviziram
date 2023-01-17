//napravicemo kada implementiramo kategorije i tako to

import { Typography,Box } from "@mui/material";
import { useNavigate } from "react-router-dom";
import RegisterForm from "../Components/Login/RegisterForm";
import { useState } from "react";

function RegisterPage() {
    const [email, setEmail] = useState("");
    const [userName, setUserName] = useState("");
    const [avatarURL, setAvatarURL] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const navigate = useNavigate();

    function handleEmailChange(event){
        setEmail(event.target.value)
    }
    function handleUserNameChange(event){
        setUserName(event.target.value)
    }
    function handleAvatarUrlChange(event){
        setAvatarURL(event.target.value)
    }
    function handlePasswordChange(event){
        setPassword(event.target.value)
    }
    function handleConfirmPasswordChange(event){
        setConfirmPassword(event.target.value)
    }
    async function tryRegister()
    {   
        try {
          const endpoint = "http://localhost:5221/Register"; // replace with the actual endpoint URL
          const body = JSON.stringify({
            username: userName,
            email: email,
            password: password,
            avatar: avatarURL
          });
          const response = await fetch(endpoint, {
            method: "POST",
            body: body,
            headers: { "Content-Type": "application/json" }
          });
          const json = await response.json();
          if (!response.ok) {
            throw json;
          }
          if(json)
          {
            handleFetchSuccess()
          }
        } catch (error) {
          throw error;
        }     
    }  
    function handleFetchSuccess(){
        navigate('/Login')
    }
    return(
        <Box className="flex-down" marginTop={'5em'}>
            <Typography variant="h1">Register</Typography>
            <div className="flex-right seperate-children-big">                     
                <RegisterForm email={email} userName={userName} avatarURL={avatarURL} password={password} confirmPassword={confirmPassword} 
                onChangeEmail={handleEmailChange} onChangeUserName={handleUserNameChange} onAvatarURLChange={handleAvatarUrlChange} onChangePassword={handlePasswordChange} onChangeConfirmPassword={handleConfirmPasswordChange}
                clickHandler={tryRegister}></RegisterForm>
            </div>     
        </Box>
 
    )
}

export default RegisterPage