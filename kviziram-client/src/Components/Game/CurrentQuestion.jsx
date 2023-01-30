import { Box, Button, FormControlLabel, LinearProgress, Radio, RadioGroup, Typography } from "@mui/material";
import { useState } from "react";
import { useEffect } from "react";

function CurrentQuestion() {
    const [selectedAnswer, setSelectedAnswer] = useState(-1);
    function handleAnswerChanged(event){
        setSelectedAnswer(event.target.value);
    }

    const [questionTimePercentage, setQuestionTimePercentage] = useState(0);
    const questionTimeSeconds = 30 //izvuci iz pitanja koje prosledis kao prop i guess

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
                <Typography className="margin" variant="h2">If monkey had balls, would he scratch them?</Typography>

                <RadioGroup value={selectedAnswer} onChange={handleAnswerChanged} sx={{alignSelf:"start"}}>
                    {/* ovde mozes da loopujes answers i napravis ova dole cuda, stavi za value index answera, sinhronizuje se sa selectedAnswerState */}
                    <FormControlLabel className="margin" value="0" control={<Radio />} label={<Typography variant="h4">Answer 1</Typography>} />
                    <FormControlLabel className="margin" value="1" control={<Radio />} label={<Typography variant="h4">Answer 2</Typography>} />
                    <FormControlLabel className="margin" value="2" control={<Radio />} label={<Typography variant="h4">Answer 3</Typography>} />
                </RadioGroup>

                <Button sx={{marginTop:"auto", alignSelf:"end"}} variant="contained" size="large">Submit Answer</Button>
            </Box>
        </Box>
    )
}

export default CurrentQuestion