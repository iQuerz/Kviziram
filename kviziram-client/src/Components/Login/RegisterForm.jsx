import { Card, Box, Typography, TextField, Button } from "@mui/material";

function RegisterForm(props)
{
    return (
        <Box maxWidth={"18em"} >
            <Card className="flex-down seperate-children-small padding">
                <Typography variant="h5"> {props.title} </Typography>
                <TextField label={"Email"} variant="outlined" value={props.email} onChange={props.onChangeEmail}></TextField>
                <TextField label={"Avatar URL"} variant="outlined" value={props.avatarURL} onChange={props.onAvatarURLChange}></TextField>
                <img className="avatar" src={props.avatarURL} alt={props.userName}></img>
                <TextField label={"UserName"} variant="outlined" value={props.userName} onChange={props.onChangeUserName}></TextField>
                <TextField label={"Passowrd"} variant="outlined" value={props.password} type={"password"} onChange={props.onChangePassword}></TextField>
                <TextField label={"Confirm Passowrd"} variant="outlined" value={props.confirmPassword} type={"password"} onChange={props.onChangeConfirmPassword}></TextField>
                <Button variant="contained" size="large" onClick={props.clickHandler}>Register</Button>
            </Card>
        </Box>
    )
}

export default RegisterForm