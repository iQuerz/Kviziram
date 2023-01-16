import { Login } from '@mui/icons-material';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';

import LoginPage from "./Pages/LoginPage";
import HomePage from './Pages/HomePage';

function App() {
    return (
        <Router>
            <div>
                <Routes>
                    <Route path="/" element={<LoginPage />} />
                    <Route path="/Home" element={<HomePage />} />
                </Routes>
            </div>
        </Router>
    )
}

export default App