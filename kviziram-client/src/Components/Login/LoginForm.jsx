import { Card, Box, Typography, TextField, Button } from "@mui/material";

function LoginForm({
    title = 'User Login',
    field1 = 'Email',
    field2 = 'Password',
    field2Type = 'password',
    clickHandler
})
{
    return (
        <Box maxWidth={"18em"} >
            <Card className="flex-down seperate-children-small padding">
                <Typography variant="h5"> {title} </Typography>
                <TextField label={field1} variant="outlined"></TextField>
                <TextField label={field2} variant="outlined" type={field2Type}></TextField>
                <Button variant="contained" size="large" onClick={clickHandler}>Login</Button>
            </Card>
        </Box>
    )
}

export default LoginForm