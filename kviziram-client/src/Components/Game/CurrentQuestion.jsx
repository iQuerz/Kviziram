import { PropaneSharp } from "@mui/icons-material";
import { Box, Button, FormControlLabel, LinearProgress, Radio, RadioGroup, Typography } from "@mui/material";
import { useState } from "react";
import { useEffect } from "react";

function CurrentQuestion(props) {
    function handleAnswerChanged(event){
        props.setAnswer(event.target.value)
    }

    const [questionTimePercentage, setQuestionTimePercentage] = useState(0);
    const questionTimeSeconds = 45 //izvuci iz pitanja koje prosledis kao prop i guess

    useEffect(() => {
        const timer = setInterval(() => {
            setQuestionTimePercentage((oldProgress) => {
                if (oldProgress === 100) {
                    //kod za isticanje vremena
                    return 0;
                }
                const diff = 100/(10 * questionTimeSeconds)
                return Math.min(oldProgress + diff, 100);
            });
            }, 100);
        return () => {
            clearInterval(timer);
        };
    }, [])
    return(
        <Box sx={{height:"100%"}}>
            <LinearProgress className="question-progressbar" variant="determinate" value={questionTimePercentage}></LinearProgress>
            <Box className="margin flex-down" sx={{ justifyContent:"space-around", height:"90%"}}>
                <Typography className="margin" variant="h2">{props.question.description}</Typography>

                <RadioGroup value={props.selectedAnswer} onChange={handleAnswerChanged} sx={{alignSelf:"start"}}>
                    {
                        props.question.options.map((option, index) => (
                            <FormControlLabel key={index} className="margin" value={index} control={<Radio />} label={<Typography variant="h4">{option}</Typography>} />
                            ) 
                        )
                    }
                </RadioGroup>

                <Button onClick={props.submitQuestion}sx={{marginTop:"auto", alignSelf:"end"}} variant="contained" size="large">Submit Answer</Button>
            </Box>
        </Box>
    )
}

export default CurrentQuestion