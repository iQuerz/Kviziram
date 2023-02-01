import LoginForm from "../Components/Login/LoginForm";

import { Typography, Box, Button } from "@mui/material";
import { Link, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import CheckList from "../Components/Login/CheckList";


function CategoryPicker(props) {

        const [categoryOptions, setCategoryOptions] = useState([]);
        const [selectedOptions, setSelectedOptions] = useState([]);
        const navigate = useNavigate();
    
        useEffect(()=>{
          tryGetCategorys();
        },[])

        function handleSetChecked(options){
            console.log(options)
            setSelectedOptions(options)
        }
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
      async function trySetPrefferdCategories() {
        try {
        const body = JSON.stringify(selectedOptions);
          const response = await fetch("http://localhost:5221/Account/me/categories/preferred/set", {
            method: "POST",
            body: body,
            headers: {
            "Content-Type": "application/json",
              SessionID : props.mySessionID
            },
          });   
          if (response.ok) {
            console.log("You set your categories...");
            handleFetchSuccess();
            }
        } catch (error) {
          console.error(error);
        }
      }
      function handleFetchSuccess(){
        navigate('/Play')
    }
      function submit(){
        trySetPrefferdCategories();
      }
   
    
    return(
        <Box className="flex-down" paddingTop={'5em'}>
            <CheckList items={categoryOptions} setChecked={handleSetChecked}>
            </CheckList> 
        <Button onClick={submit}>Submit</Button>
        </Box>
    )
}

export default CategoryPicker