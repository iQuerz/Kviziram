
import { Box, Card, Typography } from "@mui/material";
import AchievementIcon from "../Components/Achievements/AchievementIcon";

import SidebarLayout from "../Components/Layout/Sidebar/SidebarLayout";

function ProfilePage(props) {
    var achievements = [
        {
            Name: "Opalac",
            Picture: "https://static.vecteezy.com/system/resources/previews/008/693/484/original/trophy-cup-gold-cartoon-icon-vector.jpg",
        },
        {
            Name: "Upravo si rešen ćevapu glupi",
            Picture: "https://static.vecteezy.com/system/resources/previews/008/693/484/original/trophy-cup-gold-cartoon-icon-vector.jpg",
        }
    ];

    //ovo je malo teze jer se u bazi cuvaju quizzes & participatedIn relations pa idk kako ce da izgleda
    var matches = [
    ]

    return(
        <>
            <SidebarLayout>
                <Typography variant="h1">{props.mySessionID}</Typography>
                <Box className="flex-right seperate-children-medium" alignItems={"center"}>
                    <img className="avatar" src="https://hips.hearstapps.com/hmg-prod.s3.amazonaws.com/images/lionel-animals-to-follow-on-instagram-1568319926.jpg?crop=0.922xw:0.738xh;0.0555xw,0.142xh&resize=640:*"></img>
                    <Box className="flex-down">
                        <Typography variant="h1">iQuerz</Typography>
                        <Typography variant="h4">nikola@email.com</Typography>
                    </Box>
                </Box>
                
                <Card className="flex-down seperate-children-small padding margin">
                    <Typography variant="h4">Achievements</Typography>
                    <Box className="flex-list-row seperate-children-medium margin">
                        {
                            achievements.map(achievement => {
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