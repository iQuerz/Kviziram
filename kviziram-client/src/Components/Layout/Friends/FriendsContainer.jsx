import { Box, Button, Card, Divider, Typography } from "@mui/material";
import { useEffect } from "react";
import { useState } from "react";
import AddFriendModal from "./AddFriendModal";
import FriendsItem from "./FriendsItem";

function FriendsContainer(props) {
  const [isAddFriendModalOpen, setAddFriendModalOpen] = useState(false);
  const [numOfFreinds, setnumOfFreinds] = useState(0);
  const [FreindsRequests, setFreindsRequests] = useState([]);
  const [friends, setFriends] = useState([]);


  useEffect(()=>{
    tryGetMyFriends();
    tryGetMyFriendsRequests();
  },[])
  async function tryGetMyFriends() {
    try {
      const response = await fetch(
        "http://localhost:5221/Account/me/friends/all/1",
        {
          method: "GET",
          headers: {
            accept: "text/plain",
            SessionID: props.sessionID,
          },
        }
      );
      const json = await response.json();

      if (response.ok) {
        setFriends(json);
      }
    } catch (error) {
      console.error(error);
    }
  }
  async function tryGetMyFriendsRequests() {
    try {
      const response = await fetch(
        "http://localhost:5221/Account/me/friends/all/0",
        {
          method: "GET",
          headers: {
            accept: "text/plain",
            SessionID: props.sessionID,
          },
        }
      );
      const json = await response.json();

      if (response.ok) {
        setFreindsRequests(json);
      }
    } catch (error) {
      console.error(error);
    }
  }
  function handleFriends(friends) {
    setFriends(friends);
  }


  function handleAddFriendsModalOpenChange() {
    setAddFriendModalOpen(!isAddFriendModalOpen);
  }

  return (
    <>
      <Divider variant="middle" className="sidebar-divider"></Divider>
      <Box className="friends-container">
        <Typography variant="h4" fontWeight={"bold"}>
          My friends<Button color="error">{"Friend Request "}{FreindsRequests.length}</Button>
          <Button
            variant="contained"
            fullWidth={true}
            onClick={handleAddFriendsModalOpenChange}
          >
            +
          </Button>
        </Typography>

        {friends.map((friend, i) => (
          <FriendsItem key={i} account={friend} sessionID={props.sessionID}/>
        ))}
      </Box>
      <AddFriendModal
        open={isAddFriendModalOpen}
        onChange={handleAddFriendsModalOpenChange}
        sessionID={props.sessionID}
      ></AddFriendModal>
    </>
  );
}
export default FriendsContainer;
