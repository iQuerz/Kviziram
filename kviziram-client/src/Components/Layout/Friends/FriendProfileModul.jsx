
import { Box, Button, Card, Modal, Typography } from "@mui/material";
import { useEffect } from "react";
import { useState } from "react";
import ProfilePage from "../../../Pages/ProfilePage";
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
    const [myFriends, setMyFriends] = useState([]);
    const [myFriendRequests, setmyFriendRequests] = useState([]);
    const [isMyFriend, setIsMyfriend] = useState(false);
    const [sentMeARequest, setSentMeARequest] = useState(false);

    useEffect(()=>
    {   
        tryGetAchivments();
        tryGetMyFriends();
    },[])

    useEffect(()=>
    {   
        const foundF = myFriends.some(account => account.id == props.account.id)
        if (foundF){
          setIsMyfriend(true)
        }
        const foundR = myFriends.some(account => account.id == props.account.id)
        if (foundR){
          setSentMeARequest(true)
        }
    },[myFriends])
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

      useEffect(()=>{
        tryGetMyFriends();
        tryGetMyFriendRequests();
      },[])
      async function tryGetMyFriends() {
        try {
          const response = await fetch(
            "http://localhost:5221/Account/me/friends/all/1",
            {
              method: "GET",
              headers: {
                accept: "text/plain",
                SessionID: props.sessionID,
              },
            }
          );
          const json = await response.json();
    
          if (response.ok) {
            setMyFriends(json);
          }
        } catch (error) {
          console.error(error);
        }
      }
      async function tryGetMyFriendRequests() {
        try {
          const response = await fetch(
            "http://localhost:5221/Account/me/friends/all/0",
            {
              method: "GET",
              headers: {
                accept: "text/plain",
                SessionID: props.sessionID,
              },
            }
          );
          const json = await response.json();
    
          if (response.ok) {
            setmyFriendRequests(json);
          }
        } catch (error) {
          console.error(error);
        }
      }
      async function trySendFreidnRequest() {
        try {
          const response = await fetch(
            "http://localhost:5221/Account/me/friend/" + props.account.id + "/request",
            {
              method: "GET",
              headers: {
                accept: "text/plain",
                SessionID: props.sessionID,
              },
            }
          );
          const json = await response;
    
          if (response.ok) {
            console.log("Friend request sent")
          }
        } catch (error) {
          console.error(error);
        }
      }
      async function tryAnswerFreidnRequest() {
        try {
          const response = await fetch(
            "http://localhost:5221/Account/me/friend/" + props.account.id + "/request/1",
            {
              method: "GET",
              headers: {
                accept: "text/plain",
                SessionID: props.sessionID,
              },
            }
          );
          const json = await response;
    
          if (response.ok) {
            console.log("Friend request accepted")
          }
        } catch (error) {
          console.error(error);
        }
      }

      function sendFiriendRequest(){
        trySendFreidnRequest();

      }
      function answerFiriendRequest(){
        tryAnswerFreidnRequest();
        setIsMyfriend(true)
        setSentMeARequest(false)
      }

      var hisFreinds = [
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
                        <Typography variant="h1">{props.account.username}</Typography>
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
                            hisFreinds.map(achievement => {
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
                    <Button style={{display: !isMyFriend ? "none" : "initial"}}
                            onClick={sendFiriendRequest} 
                            variant="contained"
                            size="large"
                            color="success">Add </Button>
                    <Button style={{display: sentMeARequest ? "none" : "initial"}}
                            onClick={answerFiriendRequest} 
                            variant="contained"
                            size="large"
                            color="success">Accept Friend Request</Button>
                    <Button
                            onClick={props.onChange}
                            variant="contained"
                            size="large"
                            color="error" >Close</Button>
                </Card>
                    </Box>
            </Modal>
        </>
    )
}

export default FriendProfileModul