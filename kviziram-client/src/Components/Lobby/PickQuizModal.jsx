import {
    Box,
    Button,
    Card,
    CardHeader,
    MenuItem,
    Modal,
    Select,
    TextField,
    Typography,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import SelectComponent from "../Custom/SelectComponent";
import { DesktopDatePicker } from "@mui/x-date-pickers/DesktopDatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import dayjs from "dayjs";

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

function PickQuizModal(props) {
    const navigate = useNavigate();

    const [selectedQuiz, setSelectedQuiz] = useState("");
    const [category, setCategory] = useState("");
    const [achievement, setAchievement] = useState("");
    const [creator, setCreator] = useState("");
    const [inviteCode, setInviteCode] = useState("");
    const [isPublic, setIsPublic] = useState(false);
    const [order, setOrder] = useState(true);
    const [categoryOptions, setCategoryOptions] = useState([]);
    const [achievementOptions, setAchievementOptions] = useState([]);
    const [creatorOptions, setCreatorOptions] = useState([]);
    const [quizzOptions, setQuizzOptions] = useState([]);
    

    function cancelButton() {
        setSelectedQuiz("");
        props.onChange();
    }
    function startButton() {
        console.log(selectedQuiz)
        console.log(props.sessionID)
        if(selectedQuiz == "") return;

        tryCreateLobby();
        //imas ovde selectedQuiz promenljivu koja ima full objekat selected kviza :3
        //ovde fetch za pravljenje lobbyja i prosledjivanje osobe u lobby

        props.onChange();
        navigate("/Lobby");
    }
    useEffect(() => {
        tryGetCategorys();
        tryGetAchivments();
        tryGetCreators();
    }, []);
    async function tryCreateLobby()
    {   
        try {
          const endpoint = "http://localhost:5221/Game"; // replace with the actual endpoint URL
          const body = JSON.stringify({
            isSearchable : isPublic,
            quizID : selectedQuiz.id
          });
          const response = await fetch(endpoint, {
            method: "POST",
            body: body,
            headers: { "Content-Type": "application/json",
                        SessionID: props.sessionID }
          });
          const json = await response.json();
          if (!response.ok) {
            throw json;
          }
          if(json)
          {
            console.log(json);
            setInviteCode(json.inviteCode)
            window.localStorage.setItem('inviteCode', json.inviteCode)
          }
        } catch (error) {
          throw error;
        }     
    }  
    useEffect(() => {
        tryGetQuizzQuerry();
    }, [category, achievement,creator, order]);
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
                let data = [{ id: 0, name: "Any" }, ...json];
                setCategoryOptions(data);
            }
        } catch (error) {
            console.error(error);
        }
    }
    async function tryGetAchivments() {
        try {
            const response = await fetch("http://localhost:5221/Achievement/all", {
                method: "GET",
                headers: {
                    accept: "text/plain",
                },
            });
            const json = await response.json();

            if (response.ok) {
                let data = [{ id: 0, name: "Any" }, ...json];
                setAchievementOptions(data);
            }
        } catch (error) {
            console.error(error);
        }
    }
    async function tryGetCreators() {
        try {
            const response = await fetch("http://localhost:5221/Quiz/creator/all", {
                method: "GET",
                headers: {
                    accept: "text/plain",
                },
            });
            const json = await response.json();

            if (response.ok) {
                let data = json.reduce((unique, o) => {
                    if(!unique.some(obj => obj.id === o.id)) {
                        unique.push(o);
                    }
                    return unique;
                },[]);
                data = [{ id: 0, name: "Any" }, ...data];
                setCreatorOptions(data);
            }
        } catch (error) {
            console.error(error);
        }
    }
    //ovo ce se koristi ako dobijemo funkciju na backend
    async function tryGetQuizzQuerry() {
        let querry = createQuerryString();
        try {
            const endpoint =
                "http://localhost:5221/Quiz/search/" + 0 + "/" + 10 + "/q?" + querry;
            const body = JSON.stringify({
                CreatorID: null,
                AchievementID: achievement.id,
                categoryid: category.id,
                GetQuestions: false,
                OrderAsc: false,
            });
            const response = await fetch(endpoint, {
                method: "GET",
                //body: body,
                headers: { "Content-Type": "application/json" },
            });
            const json = await response.json();
            if (!response.ok) {
                throw json;
            }
            if (json) {

                setQuizzOptions(json);
            }
        } catch (error) {
            throw error;
        }
    }
    function createQuerryString() {
        let querry = "";
        if (achievement.id && achievement.id !== 0)
            querry += "AchievementID=" + achievement.id + "&";
        if (category.id && category.id !== 0)
            querry += "CategoryID=" + category.id + "&";
        if (creator.id && creator.id !== 0)
            querry += "CreatorID=" + category.id + "&";

        querry += "OrderAsc=" + order;
        return querry;
    }
    function handleCategoryChange(event) {
        setCategory(event);
    }
    function handleAchievementChange(event) {
        setAchievement(event);
    }
    function handleCreatorChange(event) {
        setCreator(event);
    }
    function handleOrderChange() {
        setOrder(!order);
    }
    function handleIsPublicChange(){
        setIsPublic(!isPublic)
    }
    return (
        <>
            <Modal open={props.open}>
                <Card sx={style} className="flex-down seperate-children-small">
                    <Typography variant="h2">{props.name}</Typography>

                    <Typography variant="h4">Choose a quiz</Typography>

                    <Box width={"90%"}>
                        <Card
                            variant="outlined"
                            sx={{ backgroundColor: "var(--white)", border: 1 }}
                        >
                            <Box className="flex-list-row">
                                <Typography fontWeight={"bold"} p={1}>
                                    Category:
                                </Typography>
                                <SelectComponent
                                    label="Category"
                                    options={categoryOptions}
                                    onChange={handleCategoryChange}
                                />
                                <Typography fontWeight={"bold"} p={1}>
                                    Achievement:
                                </Typography>
                                <SelectComponent
                                    label="Achievement"
                                    options={achievementOptions}
                                    onChange={handleAchievementChange}
                                />
                                <Typography fontWeight={"bold"} p={1}>
                                    Creator:
                                </Typography>
                                <SelectComponent
                                    label="Creator"
                                    options={creatorOptions}
                                    onChange={handleCreatorChange}
                                />
                                <Button onClick={handleOrderChange}>
                                    {order ? "ASC" : "DSC"}
                                </Button>
                                <Button onClick={handleIsPublicChange}>
                                    {isPublic ? "Public" : "Private"}
                                </Button>
                            </Box>
                            <Box
                                className="flex-right wrap margin seperate-children-small"
                                width="90%"
                            >
                                {quizzOptions.map((quizz, index) => (
                                    <Card key={index} className="padding flex-right">
                                        <Button onClick={()=>{
                                            setSelectedQuiz(quizz);
                                        }}>{quizz.name}</Button>
                                    </Card>
                                ))}
                            </Box>
                        </Card>
                    </Box>

                    <Typography variant="h6">Selected quiz:</Typography>
                    <Typography variant="h3">{selectedQuiz.name}</Typography>

                    <Box className="seperate-children-medium">
                        <Button onClick={startButton} variant="contained" size="large" color="success">
                            Start
                        </Button>
                        <Button
                            variant="contained"
                            size="large"
                            color="error"
                            onClick={cancelButton}
                        >
                            Cancel
                        </Button>
                    </Box>
                </Card>
            </Modal>
        </>
    );
}

export default PickQuizModal;
