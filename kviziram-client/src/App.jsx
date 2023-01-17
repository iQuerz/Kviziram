import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { useState } from "react";

import LoginPage from "./Pages/LoginPage";
import PlayPage from './Pages/PlayPage';
import LobbyPage from './Pages/LobbyPage';
import GamePage from './Pages/GamePage';
import CreateQuizPage from './Pages/CreateQuizPage';
import ProfilePage from './Pages/ProfilePage';
import RegisterPage from './Pages/RegisterPage';

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
                <Route path="/Play"     element={<PlayPage sessionID={sessionID}/>} />
                <Route path="/Lobby"    element={<LobbyPage />} />
                <Route path="/Game"     element={<GamePage />} />
                <Route path="/Create"   element={<CreateQuizPage />} />
                <Route path="/Profile"  element={<ProfilePage/>} />
                <Route path="/Register" element={<RegisterPage />} />
            </Routes>
        </Router>
    )
}

export default App