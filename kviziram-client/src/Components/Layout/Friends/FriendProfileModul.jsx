
import { Box, Button, Card, Modal, Typography } from "@mui/material";
import { useEffect } from "react";
import { useState } from "react";
import AchievementIcon from "../../Achievements/AchievementIcon";

const style = {
    position: "absolute",
    top: "50%",
    left: "50%",
    transform: "translate(-50%, -50%)",
    width: "50%",
    bgcolor: "var(--white)",
    boxShadow: 24,
    p: 4,
};

function FriendProfileModul(props) {
    //const [account, setAccount] = useState(props.account);
    const [achievements, setAchievements] = useState([]);
    useEffect(()=>
    {   
        console.log(props.account)
        tryGetAchivments();
    },[])
    //trenutno izvlaci sve achivments cisto radi testiranja
    async function tryGetAchivments() {
        try {
          const response = await fetch("http://localhost:5221/Achievement/all", {
            method: "GET",
            headers: {
              accept: "text/plain",
              'SessionID':  props.sessionID
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
        <><Modal open={props.open}>
            <Box sx={style} className="flex-down seperate-children-small">
                <Box className="flex-right seperate-children-medium" alignItems={"center"}>
                    <img className="avatar" src={props.account.avatar}></img>
                    <Box className="flex-down">
                        <Typography variant="h1">{props.account.name}</Typography>
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
                <Card className="seperate-children-medium">
                    <Button onClick={props.onChange}
                            variant="contained"
                            size="large"
                            color="error">Close</Button>
                </Card>
                    </Box>

            </Modal>
        </>
    )
}

export default FriendProfileModul