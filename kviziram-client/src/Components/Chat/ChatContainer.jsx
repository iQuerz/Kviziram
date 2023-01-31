import { Card, TextField, useRadioGroup, Button, Typography } from "@mui/material"
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
        setAllMsg(json.reverse())
      }
    } catch (error) {
      console.error(error);
    }
  }

  function handleSendMsg() {
    props.sendMsg(currentMsg.current.value)
    currentMsg.current.value = "";
  }
  return (
    <>
      <Box sx={{alignItems:"start", flexDirection:"column", marginBottom:"2em", overflow:"auto"}}>
        {
         allMsg.map((msg,index) => {
          return (<Typography key={index}>{msg}</Typography>)
         })
        }
      </Box>

      <Box className="flex-right">
        <TextField label="Message" inputRef={currentMsg} fullWidth={true} onKeyDown={
          function(event){
            if (event.key === 'Enter')
              handleSendMsg();
          }
        }/>
        <Button onClick={handleSendMsg}>Send</Button>
      </Box>
    </>
  )
}

export default ChatContainer