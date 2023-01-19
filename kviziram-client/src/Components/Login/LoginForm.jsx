import { Card, Box, Typography, TextField, Button } from "@mui/material";

function LoginForm(props)
{
    return (
        <Box maxWidth={"18em"} >
            <Card className="flex-down seperate-children-small padding">
                <Typography variant="h5"> {props.title} </Typography>
                <TextField label={props.field1} variant="outlined" value={props.value1} onChange={props.onChangeField1}></TextField>
                <TextField label={props.field2} variant="outlined" value={props.value2} type={props.field2Type} onChange={props.onChangeField2}></TextField>
                <Button variant="contained" size="large" onClick={props.clickHandler}>Login</Button>
            </Card>
        </Box>
    )
}

export default LoginForm