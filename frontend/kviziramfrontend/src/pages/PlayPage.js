import { Link } from "react-router-dom";
import { useState } from "react";
import CodeOverlay from "../components/CodeOverlay";
import Backdrop from "../components/Backdrop";
//import classes from './PlayPage.modul.css';

function PlayPage() {
  const [overlayIsOpen, setOverlayIsopen] = useState(false);

  function overlayHandler() {
    setOverlayIsopen(true);
  }
  function closeOverlayHandler(){
    setOverlayIsopen(false)
  }
  return (
    <div>
      <h1>Play Quizz</h1>
      <div>
        <button className="btn" onClick={overlayHandler}>Join with code</button>
      </div>
      <div>
      <button className="btn" onClick={overlayHandler}>Host game</button>
      </div>
      <div>
      <button className="btn" onClick={overlayHandler}>Join Lobby</button>
      </div>
      {overlayIsOpen && <CodeOverlay onCancel={closeOverlayHandler} onConfirm={closeOverlayHandler}/>}
      {overlayIsOpen && <Backdrop onClick={closeOverlayHandler}/>}
    </div>
  );
}

export default PlayPage;
