import './CodeOverlay.modul.css';

function CodeOverlay(props) {

  function cancleHandler(){
    props.onCancel();
  }
  function confirmHandler(){
    props.onConfirm();
  }
  return (
    <div className="modal">
        <p>
          Code:
          <input type="text" name="code" />
        </p>
        <button className='btn btn--alt' onClick={confirmHandler}>Confirm</button>
        <button className='btn'onClick={cancleHandler}>Cancel</button>
    </div>
  );
}
export default CodeOverlay;
