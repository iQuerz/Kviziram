
import { Box, Card, Typography } from "@mui/material";
import { useEffect } from "react";
import { useState } from "react";
import AchievementIcon from "../Components/Achievements/AchievementIcon";

import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";

function ProfilePage(props) {
    //const [account, setAccount] = useState(props.account);
    const [achievements, setAchievements] = useState([]);
    useEffect(()=>
    {   
        console.log("Loading Achivments...");
        tryGetAchivments();
    },[])
    //trenutno izvlaci sve achivments cisto radi testiranja
    async function tryGetAchivments() {
        try {
          const response = await fetch("http://localhost:5221/Achievement/all", {
            method: "GET",
            headers: {
              accept: "text/plain",
              'SessionID':  props.mySessionID
            },
          });
          const json = await response.json();
    
          if (response.ok) {
            setAchievements(json); 
          }
        } catch (error) {
          console.error(error);
        }
      }
    var friends = [
        
    ]

    //ovo je malo teze jer se u bazi cuvaju quizzes & participatedIn relations pa idk kako ce da izgleda
    var matches = [
    ]

    return(
        <>
            <SidebarLayout  sessionID={props.mySessionID}>
                <Typography variant="h1">{props.mySessionID}</Typography>
                <Box className="flex-right seperate-children-medium" alignItems={"center"}>
                    <img className="avatar" src={props.account.avatar}></img>
                    <Box className="flex-down">
                        <Typography variant="h1">{props.account.username}</Typography>
                        <Typography variant="h4">{props.account.email}</Typography>
                    </Box>
                </Box>
                
                <Card className="flex-down seperate-children-small padding margin">
                    <Typography variant="h4">Achievements</Typography>
                    <Box className="flex-list-row seperate-children-medium margin">
                        {
                            achievements.map((achievement, index) => {
                                return(<AchievementIcon key={index} achievement={achievement}/>)
                            })
                        }
                    </Box>
                </Card>

                <Card className="flex-down seperate-children-small padding margin">
                    <Typography variant="h4">Friends</Typography>
                    <Box className="flex-list-row seperate-children-big margin">
                        {
                            friends.map(achievement => {
                                return(<AchievementIcon achievement={achievement}/>)
                            })
                        }
                    </Box>
                </Card>

                <Card className="flex-down seperate-children-small padding margin">
                    <Typography variant="h4">Recent matches</Typography>
                    <Box className="flex-list-row seperate-children-big margin">
                        {
                            matches.map(achievement => {
                                return(<AchievementIcon achievement={achievement}/>)
                            })
                        }
                    </Box>
                </Card>
            </SidebarLayout>
        </>
    )
}

export default ProfilePage