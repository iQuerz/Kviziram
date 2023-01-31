
import { Box, Button, Card, TextField, Typography } from "@mui/material";
import { useEffect } from "react";
import { useState } from "react";
import SelectComponent from "../Components/Custom/SelectComponent";

function AdminPage() {
    let [categoryName, setCatName] = useState("");
    function onCategoryNameChange(event){
        setCatName(event.target.value);
    }

    function addCategory(){
        console.log(categoryName)
        tryAddCategory();
    }
    
    async function tryAddCategory(){
        try {
            const Category = {name: categoryName}
            const body = JSON.stringify(Category);
            const response = await fetch('http://localhost:5221/Category', {
                method: 'POST',
                body: body,
                headers: { "Content-Type": "application/json"}
            });
            const json = await response.json();
            
            if(response.ok)
            {
                console.log("Category added")
                setCatName("");
            }
        } catch (error) {
            //setErrorMsg(error)
            console.error(error);
        }
    }
    let [achievementName, setAcName] = useState("");
    let [desc, setDesc] = useState("");
    let [pictureUrl, setPictureUrl] = useState("");
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
        tryAddAchievement();
    }
    async function tryAddAchievement(){
        try {
            const Achievement = {  name: achievementName,
            description: desc,
            picture: pictureUrl}
            const body = JSON.stringify(Achievement);
            const response = await fetch('http://localhost:5221/Achievement', {
                method: 'POST',
                body: body,
                headers: { "Content-Type": "application/json"}
            });
            const json = await response.json();
            
            if(response.ok)
            {
                console.log("Achivment added")
                setAcName("");
                setDesc("");
                setPictureUrl("");
            }
        } catch (error) {
            //setErrorMsg(error)
            console.error(error);
        }
    }
    let [adName, setAdname] = useState("");
    let [companyName, setCompanyName] = useState("");
    let [link, setLink] = useState("");
    let [companyPicURL, setCompanyPicURL] = useState("");

    function onAdNameChange(event){
        setAdname(event.target.value);
    }
    function onCompanyNameChange(event){
        setCompanyName(event.target.value);
    }
    function onLinkChange(event){
        setLink(event.target.value);
    }
    function onCompanyPicURL(event){
        setCompanyPicURL(event.target.value);
    }

    function addAd(){
        tryAddAd();
    }
    async function tryAddAd(){
        try {
            const Ad = {  name: adName,
                                companyName: companyName,
                                link: link,
                                url: companyPicURL}
            const body = JSON.stringify(Ad);
            const response = await fetch('http://localhost:5221/Ad', {
                method: 'POST',
                body: body,
                headers: { "Content-Type": "application/json"}
            });
            const json = await response.json();
            
            if(response.ok)
            {
                console.log("Ad added")
                setAdname("");
                setCompanyName("");
                setLink("");
                setCompanyPicURL("");
            }
        } catch (error) {
            //setErrorMsg(error)
            console.error(error);
        }
    }
    let [categoryOptions, setCategoryOptions] = useState([]);
    let [adOptions, setAdOptions] = useState([]);
    let [selectedCategory, setSelectedCategory] = useState("");
    let [selectedAd, setSelectedAd] = useState("");
    let [paidAmount, setPaidAmount] = useState("");

    function onLinkChange(event){
        setLink(event.target.value);
    }
    function onCompanyPicURL(event){
        setCompanyPicURL(event.target.value);
    }
    function onPaiedChange(event){
        setPaidAmount(event.target.value);
    }

    function addConnection(){
        tryAddConnection();
    }
    async function tryAddConnection(){
        try {
            const response = await fetch('http://localhost:5221/Ad/'+selectedAd.id+"/connect/category/"+selectedCategory.id, {
                method: 'POST',
                body: paidAmount,
                headers: { "Content-Type": "application/json"}
            });
            const json = await response.json();
            
            if(response.ok)
            {
                console.log("Ad connected.")
                setAdname("");
                setCompanyName("");
                setLink("");
                setCompanyPicURL("");
            }
        } catch (error) {
            //setErrorMsg(error)
            console.error(error);
        }
    }
    useEffect(() =>{
        tryGetCategorys();
        tryGetAd();
    },[])

    async function tryGetCategorys() {
        try {

          const response = await fetch("http://localhost:5221/Category/all", {
            method: "GET",
            headers: {
              accept: "text/plain",
            },
          });
          const json = await response.json();
    
          if (response.ok) {
            setCategoryOptions(json); //predaje se sessionID app komponenti
          }
        } catch (error) {
          console.error(error);
        }
      }
      async function tryGetAd() {
        try {
          const response = await fetch("http://localhost:5221/Ad/all", {
            method: "GET",
            headers: {
              accept: "text/plain",
            },
          });
          const json = await response.json();
    
          if (response.ok) {
            setAdOptions(json); //predaje se sessionID app komponenti
          }
        } catch (error) {
          console.error(error);
        }
      }
      function handleCategoryChange(value){
        setSelectedCategory(value);
      }
      function handleAdChange(value){
        setSelectedAd(value);
      }
    return(
        <>
            <Box className="flex-right seperate-children-big padding">

                <Card className="flex-down seperate-children-small padding">
                    <Typography>Add Category</Typography>
                    <TextField label="Name" value={categoryName} onChange={onCategoryNameChange}></TextField>
                    <Button type="contained" onClick={addCategory} >Add</Button>
                </Card>

                <Card className="flex-down seperate-children-small padding">
                    <Typography>Add Achievement</Typography>
                    <TextField label="Name" value={achievementName} onChange={onAchievementNameChange}></TextField>
                    <TextField label="Desc" value={desc} onChange={onDescriptionChange}></TextField>
                    <TextField label="Picture URL" value={pictureUrl} onChange={onUrlChange}></TextField>
                    <Button type="contained" onClick={addAchievement}>Add</Button>
                </Card>

                <Card className="flex-down seperate-children-small padding">
                    <Typography>Add Ad</Typography>
                    <TextField label="Name" value={adName} onChange={onAdNameChange}></TextField>
                    <TextField label="Company" value={companyName} onChange={onCompanyNameChange}></TextField>
                    <TextField label="Link" value={link} onChange={onLinkChange}></TextField>
                    <TextField label="url" value={companyPicURL} onChange={onCompanyPicURL}></TextField>
                    <Button type="contained" onClick={addAd}>Add</Button>
                </Card>

                <Card className="flex-down seperate-children-small padding">
                    <Typography>Connect Ad to Category</Typography>
                    <TextField label="Ammout paid" value={paidAmount} onChange={onPaiedChange}></TextField>
                    <SelectComponent
                    label="Category"
                    options={categoryOptions}
                    onChange={handleCategoryChange}
                    />
                    <SelectComponent
                    label="Ad"
                    options={adOptions}
                    onChange={handleAdChange}
                    />

                    <Button type="contained" onClick={addConnection}>Add</Button>
                </Card>

            </Box>
        </>
    )
}

export default AdminPage