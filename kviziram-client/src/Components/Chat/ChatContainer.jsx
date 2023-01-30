import { Card, TextField, useRadioGroup, Button } from "@mui/material"
import { Box } from "@mui/system"
import { useEffect, useRef, useState } from "react"
import MsgLine from "./MsgLine";

function ChatContainer(props) {
    const [allMsg, setAllMsg] = useState([]);
    const currentMsg = useRef("");

    useEffect(() => {
        tryGetChat();
    }, [props.msgRecived, props.inviteCode])

    async function tryGetChat() {
        try {
          const response = await fetch(
            "http://localhost:5221/Game/" + props.inviteCode + "/chat/0/100",
            {
              method: "GET",
              headers: {
                accept: "text/plain",
                SessionID: props.sessionId,
              },
            }
          );
          const json = await response.json();
    
          if (response.ok) {
            //console.log(json)

            setAllMsg(json)
          }
        } catch (error) {
          console.error(error);
        }
      }

      function handleSendMsg(){
        props.sendMsg(currentMsg.current.value)
      }
    return(
        <Box className="chat-container">
            <Box>
                    {
                        allMsg.map((msg, index) => {
                            return (<MsgLine  msg={msg} key={index} ></MsgLine>)
                        })
                    }
            </Box>
            <TextField label="Message" inputRef={currentMsg}> </TextField>
            <Button onClick={handleSendMsg}> Send</Button>
        </Box>
    )
}

export default ChatContainer