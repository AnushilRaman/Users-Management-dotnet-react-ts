import { useState } from "react";
import { Control, Controller } from "react-hook-form";

interface IProps {
    usernames: string[];
    control: Control<any, any>;
    name: string;
    error?: string;
}
const UserNameComboBox = ({ usernames, control, name, error }: IProps) => {
    const [inputValue, setInputValue] = useState<string>('');
    const [showComboBox, setShowComboBox] = useState<boolean>(false);

    const renderTopRow = () => {
        if (error) {
            return <span className="text-red-600 font-semibold">{error}</span>
        }
        else {
            return <span className="font-semibold">To</span>
        }
    }
    let userNamesToShow = inputValue ? usernames?.filter((x) => x.includes(inputValue)) : usernames;
    const dynamicClassNames = error ? 'border-red-500 rounded-lg' : 'border-[#754eb477]';

    return (
        <div className="px-4 my-2 w-9/12">
            {renderTopRow()}
            <Controller name={name} control={control} render={({ field }) => (
                <>
                    <input className={dynamicClassNames}
                        type="text" autoComplete="off"
                        value={inputValue} onChange={(event) => {
                            if (!showComboBox)
                                setShowComboBox(true);
                            let { value } = event.target
                            setInputValue(value);
                            field.onChange(value);
                            if (usernames.includes(value))
                                setShowComboBox(false);
                        }}
                        onFocus={() => { setShowComboBox(true) }}
                    />
                    {
                        showComboBox && userNamesToShow.length > 0 ? (
                            <div className="relative">
                                <div className="absolute p-2 top-0 left-0 right-0 bg-gray-200">
                                    {
                                        userNamesToShow.map((item, index) => (
                                            <div key={index}
                                                className="p-1 m-2 bg-white font-semibold"
                                                onClick={() => {
                                                    setInputValue(item);
                                                    setShowComboBox(false);
                                                    field.onChange(item)
                                                }}
                                            >
                                                {item}
                                            </div>
                                        ))
                                    }
                                </div>
                            </div>
                        ) : null
                    }
                </>
            )}
            />
        </div>
    )
}

export default UserNameComboBox
