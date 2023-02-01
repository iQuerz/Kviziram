import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { useEffect, useState } from "react";

import LoginPage from "./Pages/LoginPage";
import PlayPage from './Pages/PlayPage';
import LobbyPage from './Pages/LobbyPage';
import GamePage from './Pages/GamePage';
import CreateQuizPage from './Pages/CreateQuizPage';
import ProfilePage from './Pages/ProfilePage';
import RegisterPage from './Pages/RegisterPage';
import AdminPage from './Pages/AdminPage';
import SidebarLayout from './Components/Layout/Sidebar/SidebarLayout';
import CategoryPicker from './Pages/CategoryPicker';

var globalAccount;
var globalSessionID;

function App() {
    const [sessionID, setSessionID] = useState("")
    const [myAccount, setMyAccount] = useState("");
    const [myAd, setMyAd] = useState("");
    const [hubConnection, setHubConnection] = useState();

    useEffect(()=>{
        tryGetAccount();
    },[sessionID])

    async function tryGetAccount() {
        try {
          const response = await fetch("http://localhost:5221/Account/me", {
            method: "GET",
            headers: {
              accept: "text/plain",
              'SessionID':  sessionID
            },
          });
          const json = await response.json();
    
          if (response.ok) {
            setMyAccount(json); 
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
              'SessionID':  sessionID
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
              console.log(json)
              setMyAd(json); 
            }

          }
        } catch (error) {
          console.error(error);
        }
      }
    function handleSessionIDChange(sessionID){
        setSessionID(sessionID);
        localStorage.setItem('sessionID', sessionID);
    }
    function handleHubConnectionChange(hub)
    {
      console.log(hub)
      setHubConnection(hub);
    }

    return (
        <Router>
            <Routes>     
                <Route path="/"         element={<LoginPage onSessionIDChange={handleSessionIDChange} />} />
                <Route path="/Play"     element={<PlayPage mySessionID={sessionID}/>} />
                <Route path="/Lobby"    element={<LobbyPage mySessionID={sessionID} onHubChange={handleHubConnectionChange}/>} />
                <Route path="/Create"   element={<CreateQuizPage mySessionID={sessionID}/>} />
                <Route path="/Profile"  element={<ProfilePage mySessionID={sessionID} account={myAccount}/>} />
                <Route path="/Register" element={<RegisterPage />} />
                <Route path="/Admin"    element={<AdminPage />} />
                <Route path="/Category" element={<CategoryPicker mySessionID={sessionID}/>} />
                <Route path="/Game"     element={<GamePage mySessionID={sessionID} hubConnection={hubConnection} />} />
            </Routes>
        </Router>
    )
}

export default App