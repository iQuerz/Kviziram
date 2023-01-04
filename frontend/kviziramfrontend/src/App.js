import { Routes, Route } from "react-router-dom";
import CreatePage from "./pages/CreatePage";
import HomePage from "./pages/HomePage";
import PlayPage from "./pages/PlayPage";
import ProfilePage from "./pages/ProfilePage";
import MainNavigation from "./components/LayOut/MainNavigation";
import CodeOverlay from "./components/CodeOverlay";
import "./App.modul.css"
function App() {
  return (
    <div className="container">
      <MainNavigation/>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/Play-Page" element={<PlayPage />} />
        <Route path="/Create-Page" element={<CreatePage />} />
        <Route path="/Profile-Page" element={<ProfilePage />} />
      </Routes>
    </div>
  );
}

export default App;
