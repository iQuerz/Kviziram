
import { Box, Button, Card, TextField, Typography } from "@mui/material";
import { useState } from "react";

function AdminPage() {
    let [categoryName, setCatName] = useState();
    function onCategoryNameChange(event){
        setCatName(event.target.value);
    }

    function addCategory(){

    }
    

    let [achievementName, setAcName] = useState();
    let [desc, setDesc] = useState();
    let [pictureUrl, setPictureUrl] = useState();
    function onAchievementNameChange(event){
        setAcName(event.target.value);
    }
    function onDescriptionChange(event){
        setDesc(event.target.value);
    }
    function onUrlChange(event){
        setPictureUrl(event.target.value);
    }

    function addAchievement(){

    }

    return(
        <>
            <Box className="flex-right seperate-children-big padding">
                <Card className="flex-down seperate-children-small padding">
                    <Typography>Add Category</Typography>
                    <TextField label="Name" onChange={onCategoryNameChange}></TextField>
                    <Button type="contained" onClick={addCategory}>Add</Button>
                </Card>
                <Card className="flex-down seperate-children-small padding">
                    <Typography>Add Achievement</Typography>
                    <TextField label="Name" onChange={onAchievementNameChange}></TextField>
                    <TextField label="Desc" onChange={onDescriptionChange}></TextField>
                    <TextField label="Picture URL" onChange={onUrlChange}></TextField>
                    <Button type="contained" onClick={addAchievement}>Add</Button>
                </Card>
            </Box>
        </>
    )
}

export default AdminPage