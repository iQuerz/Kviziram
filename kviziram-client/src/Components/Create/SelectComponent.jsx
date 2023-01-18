import React, { useState } from 'react';
import { Select, MenuItem, FormControl } from '@mui/material';

function SelectComponent(props) {
    const [selectedOption, setSelectedOption] = useState(props.options[0]);

    const handleChange = (event) => {
        setSelectedOption(event.target.value);
        props.onChange && props.onChange(event.target.value)
    };
    return (
        <FormControl>
            <Select
                labelId="demo-simple-select-label"
                id="demo-simple-select"
                value={selectedOption}
                label={props.label}
                onChange={handleChange}
            >
                {props.options.map((option) => (
                    <MenuItem key={option} value={option}>
                        {option}
                    </MenuItem>
                ))}
            </Select>
        </FormControl>
    );
}

export default SelectComponent;
