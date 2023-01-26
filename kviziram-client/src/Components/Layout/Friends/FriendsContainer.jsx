import { Box, Button, Card, Divider, Typography } from "@mui/material";
import { useEffect } from "react";
import { useState } from "react";
import AddFriendModal from "./AddFriendModal";
import FriendsItem from "./FriendsItem";

function FriendsContainer(props) {
  const [isAddFriendModalOpen, setAddFriendModalOpen] = useState(false);
  const [numOfFreinds, setnumOfFreinds] = useState(0);
  const [friends, setFriends] = useState([]);


  useEffect(()=>{
    console.log("Loading Freinds...");
    console.log(props.sessionID)
    tryGetMyFriends();
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
          My friends{" "}
          <Button
            variant="contained"
            fullWidth="true"
            onClick={handleAddFriendsModalOpenChange}
          >
            +
          </Button>
        </Typography>

        {friends.map((friend, i) => (
          <FriendsItem key={i} avatar={friend.avatar} name={friend.username} status={friend.status} />
        ))}
      </Box>
      <AddFriendModal
        open={isAddFriendModalOpen}
        onChange={handleAddFriendsModalOpenChange}
      ></AddFriendModal>
    </>
  );
}
export default FriendsContainer;
