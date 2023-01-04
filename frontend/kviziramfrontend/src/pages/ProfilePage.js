import React from 'react';
import './ProfilePage.modul.css';

function ProfilePage() {
  //const { name, avatarUrl, bio } = props;
    const name = "Place Holder"
    const bio = "A long description, is a way to provide long alternative text for non-text elements, such as images. Generally, alternative text exceeding 250 characters, which cannot be made more concise without making it less descriptive or meaningful, should have a long description."
    const avatarUrl = "https://images.unsplash.com/photo-1615751072497-5f5169febe17?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxzZWFyY2h8Mnx8Y3V0ZSUyMGRvZ3xlbnwwfHwwfHw%3D&w=1000&q=80"
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