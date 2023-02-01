import React, { useState } from 'react';

import {Button, FormControl, FormGroup,FormControlLabel,Checkbox,Card, Typography  } from '@mui/material';

const CheckList = (props) => {
  const [checkedItems, setCheckedItems] = useState([]);

  const handleCheck = (e) => {
    if (e.target.checked) {
      setCheckedItems([...checkedItems, e.target.value]);
      props.setChecked([...checkedItems, e.target.value]);
    } else {
      setCheckedItems(checkedItems.filter((item) => item !== e.target.value));
      props.setChecked(checkedItems.filter((item) => item !== e.target.value));
    }
  };


  return (
    <form >
      <Card>
        <Typography variant='h6'>Let us know what kind of quizzes you preffer</Typography>
        <FormGroup className="flex-right wrap margin seperate-children-small">
          {props.items.map((item, index) => (
            <FormControlLabel key={index} control={<Checkbox key={index}value={item.id} onChange={handleCheck}/>} label={item.name} />
          ))}
        </FormGroup>
      </Card>
    </form>
  );
};

export default CheckList;
