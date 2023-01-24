import React, { useState } from 'react';
import { Select, MenuItem, FormControl } from '@mui/material';
import { useEffect } from 'react';

function SelectComponent(props) {
    const [selectedOption, setSelectedOption] = useState('');

    useEffect(() => {
        //setSelectedOption(props.options[0])
    },)
    function printDiagnostics(){
        console.log("componet is " + selectedOption)
    }
    const handleChange = (event) => {
        setSelectedOption(event.target.value);
        props.onChange && props.onChange(event.target.value)
    };
    return (
        <FormControl>
            <Select
                labelId="demo-simple-select-label"
                id="demo-simple-select"
                label={props.label}
                value={selectedOption}
                onChange={handleChange}
            >
                {props.options.map((option) => (
                    <MenuItem key={option.id} value={option}>
                        {option.name}
                    </MenuItem>
                ))}
            </Select>
        </FormControl>
    );
}

export default SelectComponent;
