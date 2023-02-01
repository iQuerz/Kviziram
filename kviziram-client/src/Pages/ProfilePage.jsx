
import { Box, Card, Typography } from "@mui/material";
import { useEffect } from "react";
import { useState } from "react";
import AchievementIcon from "../Components/Achievements/AchievementIcon";
import MatchIcon from "../Components/Game/MatchIcon";

import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";

function ProfilePage(props) {
    //const [account, setAccount] = useState(props.account);
    const [achievements, setAchievements] = useState([]);
    const [matches, setMatches] = useState([]);

    useEffect(()=>
    {   
        tryGetAchivments();
        tryGetMatches();
    },[])
    //trenutno izvlaci sve achivments cisto radi testiranja
    async function tryGetAchivments() {
        try {
        console.log(props.mySessionID)
          const response = await fetch("http://localhost:5221/Account/"+props.account.id+"/achievements/get", {
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
        }
        catch (error) {
            console.error(error);
        }
    }
    async function tryGetMatches(){
        try {
            let body = {
                fromDate:"2022-02-01T15:48:49.968Z",
                toDate: "2030-02-01T15:48:49.968Z"
            }
            const response = await fetch("http://localhost:5221/Match/search/history/0/10/q", {
                method: "POST",
                headers: {
                    accept: "text/plain",
                    'SessionID':  localStorage.getItem('sessionID')
                },
            });
            const json = await response.json();
    
            if (response.ok) {
                console.log("matches:",json);
                setMatches(json); 
            }
        }
        catch (error) {
            console.error(error);
        }
    }
    var friends = [
        
    ]

    //ovo je malo teze jer se u bazi cuvaju quizzes & participatedIn relations pa idk kako ce da izgleda

    return(
        <>
            <SidebarLayout  sessionID={props.mySessionID}>
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
{/* 
                <Card className="flex-down seperate-children-small padding margin">
                    <Typography variant="h4">Friends</Typography>
                    <Box className="flex-list-row seperate-children-big margin">
                        {
                            friends.map(achievement => {
                                return(<AchievementIcon achievement={achievement}/>)
                            })
                        }
                    </Box>
                </Card> */}

                <Card className="flex-down seperate-children-small padding margin">
                    <Typography variant="h4">Recent matches</Typography>
                    <Box className="flex-list-row seperate-children-big margin">
                        {
                            matches.map((match,index) => {
                                return(<MatchIcon match={match} key={index}></MatchIcon>)
                            })
                        }
                    </Box>
                </Card>
            </SidebarLayout>
        </>
    )
}

export default ProfilePage