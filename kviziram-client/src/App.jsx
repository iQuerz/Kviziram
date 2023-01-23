import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { useState } from "react";

import LoginPage from "./Pages/LoginPage";
import PlayPage from './Pages/PlayPage';
import LobbyPage from './Pages/LobbyPage';
import GamePage from './Pages/GamePage';
import CreateQuizPage from './Pages/CreateQuizPage';
import ProfilePage from './Pages/ProfilePage';
import RegisterPage from './Pages/RegisterPage';
import AdminPage from './Pages/AdminPage';

function App() {
    const [sessionID, setSessionID] = useState("")

    function handleSessionIDChange(sessionID){
        console.log("from app page :" + sessionID)
        setSessionID(sessionID);
    }
    return (
        <Router>
            <Routes>
                <Route path="/"         element={<LoginPage onSessionIDChange={handleSessionIDChange} />} />
                <Route path="/Play"     element={<PlayPage mySessionID={sessionID}/>} />
                <Route path="/Lobby"    element={<LobbyPage />} />
                <Route path="/Game"     element={<GamePage />} />
                <Route path="/Create"   element={<CreateQuizPage mySessionID={sessionID}/>} />
                <Route path="/Profile"  element={<ProfilePage mySessionID={sessionID}/>} />
                <Route path="/Register" element={<RegisterPage />} />
                <Route path="/Admin" element={<AdminPage />} />
            </Routes>
        </Router>
    )
}

export default App