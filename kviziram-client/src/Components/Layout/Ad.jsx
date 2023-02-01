import { PropaneSharp } from "@mui/icons-material";
import { Box, Button, Typography } from "@mui/material";
import { useEffect,useState} from "react";

function SideBar(props) {
    const [myAd, setMyAd] = useState({
        id:0,
        name:"Kviziram",
        companyName:"Samo Ventilatori TM",
        link:"https://www.youtube.com/watch?v=dQw4w9WgXcQ",
        url:"https://media.discordapp.net/attachments/946153561102884932/965300367485206538/logo-SI-white.png"
      });
    useEffect(() => {
        tryGetAd();
        if(myAd.id != 0)
            trySetAdViewed();
    }, [])
    function navigateTolink(){
        if(myAd.id != 0)
            trySetAdClicked();
        window.open(myAd.link);
    }
    function blockAd(){
        setMyAd({
            id:0,
            name:"Kviziram",
            companyName:"Samo Ventilatori TM",
            link:"https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            url:"https://media.discordapp.net/attachments/946153561102884932/965300367485206538/logo-SI-white.png"
          });
        trySetAdBlocked();
    }
    async function trySetAdViewed() {
        try {
          const response = await fetch("http://localhost:5221/Ad/"+myAd.id+"/me/viewed", {
            method: "GET",
            headers: {
              accept: "text/plain",
              'SessionID':  props.mySessionID
            },
          });

    
          if (response.ok) {

          }
        } catch (error) {
          console.error(error);
        }
      }
      async function trySetAdClicked() {
        try {
          const response = await fetch("http://localhost:5221/Ad/"+myAd.id+"/me/clicked", {
            method: "GET",
            headers: {
              accept: "text/plain",
              'SessionID':  props.mySessionID
            },
          });
    
          if (response.ok) {

          }
        } catch (error) {
          console.error(error);
        }
      }
      async function trySetAdBlocked() {
        try {
            console.log(props.mySessionID)
          const response = await fetch("http://localhost:5221/Ad/"+myAd.id+"/me/block", {
            method: "GET",
            headers: {
              accept: "text/plain",
              'SessionID':  props.mySessionID
            },
          });
    
          if (response.ok) {

          }
        } catch (error) {
          console.error(error);
        }
      }
      async function tryGetAd() {
        try {
          const response = await fetch("http://localhost:5221/Account/me/ads/recommended", {
            method: "GET",
            headers: {
              accept: "text/plain",
              'SessionID':  props.mySessionID
            },
          });
  
          if (response.ok) {
            if(response.status == 204)
            {
              setMyAd({
                id:0,
                name:"Kviziram",
                companyName:"Samo Ventilatori TM",
                link:"https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                url:"https://media.discordapp.net/attachments/946153561102884932/965300367485206538/logo-SI-white.png"
              })
            }
            else
            {
              const json = await response.json();
              setMyAd(json); 
            }

          }
        } catch (error) {
          console.error(error);
        }
      }
    return(
        <Box className="ad">
            <img src={myAd.url} onClick={navigateTolink} >
            </img>
            <Button style={{display: myAd.id == 0 ? "none" : "initial"}} onClick={blockAd} color="error">X</Button>
        </Box>
    )
}

export default SideBar