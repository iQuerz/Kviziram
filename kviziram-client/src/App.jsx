import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

import LoginPage from "./Pages/LoginPage";
import PlayPage from './Pages/PlayPage';
import LobbyPage from './Pages/LobbyPage';
import GamePage from './Pages/GamePage';
import CreateQuizPage from './Pages/CreateQuizPage';
import ProfilePage from './Pages/ProfilePage';

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/"         element={<LoginPage />} />
                <Route path="/Play"     element={<PlayPage />} />
                <Route path="/Lobby"    element={<LobbyPage />} />
                <Route path="/Game"     element={<GamePage />} />
                <Route path="/Create"   element={<CreateQuizPage />} />
                <Route path="/Profile"  element={<ProfilePage/>} />
            </Routes>
        </Router>
    )
}

export default App