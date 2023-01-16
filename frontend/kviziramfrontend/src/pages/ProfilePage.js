import React from 'react';
import './ProfilePage.modul.css';

function ProfilePage() {
  //const { name, avatarUrl, bio } = props;
    const name = "Place Holder"
    const bio = "A long description, is a way to provide long alternative text for non-text elements, such as images. Generally, alternative text exceeding 250 characters, which cannot be made more concise without making it less descriptive or meaningful, should have a long description."
    const avatarUrl = "https://static.nationalgeographic.co.uk/files/styles/image_3200/public/comedy-wildlife-awards-squirel-stop.jpg?w=710&h=530"
    return (
    <div className="profile">
      <img className="avatar" src={avatarUrl} alt={name} />
      <div className="info">
        <h1>{name}</h1>
        <p>{bio}</p>
      </div>
    </div>
  );
}

export default ProfilePage;