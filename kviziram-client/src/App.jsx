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

var globalAccount;
var globalSessionID;

function App() {
    const [sessionID, setSessionID] = useState("")
    const [myAccount, setMyAccount] = useState("");
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
                <Route path="/Game"     element={<GamePage mySessionID={sessionID} hubConnection={hubConnection} />} />
            </Routes>
        </Router>
    )
}

export default App