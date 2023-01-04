//overlay koj kada koj ti onemoguci da kliknes na nesto u pozadini
import "./BackDrop.modul.css"
function Backdrop(props) {
    return <div className="backdrop" onClick={props.onClick}/>;   
}

export default Backdrop

