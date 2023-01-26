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

function App() {
    const [sessionID, setSessionID] = useState("")
    const [myAccount, setMyAccount] = useState("");

    useEffect(()=>{
        console.log("sessionID changes");
        tryGetAccount();
    },[sessionID])

    async function tryGetAccount() {
        try {
          const response = await fetch("http://localhost:5221/Account?uID=1e60c21d-3b80-4645-95e2-7c9ce8b62618", {
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
    }

    return (
        <Router>
            <Routes>     
                <Route path="/"         element={<LoginPage onSessionIDChange={handleSessionIDChange} />} />
                <Route path="/Play"     element={<PlayPage mySessionID={sessionID}/>} />
                <Route path="/Lobby"    element={<LobbyPage mySessionID={sessionID}/>} />
                <Route path="/Create"   element={<CreateQuizPage mySessionID={sessionID}/>} />
                <Route path="/Profile"  element={<ProfilePage mySessionID={sessionID} account={myAccount}/>} />
                <Route path="/Register" element={<RegisterPage />} />
                <Route path="/Admin"    element={<AdminPage />} />
                <Route path="/Game"     element={<GamePage />} />
            </Routes>
        </Router>
    )
}

export default App