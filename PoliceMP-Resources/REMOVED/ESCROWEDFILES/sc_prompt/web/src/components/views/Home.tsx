import React, { useEffect, useState } from 'react';
import { render } from 'react-dom'
import { BrowserRouter, Switch, Route, useHistory } from 'react-router-dom';
import './Home.scss'
import { useNuiEvent } from '../../hooks/useNuiEvent';
import { fetchNui } from '../../utils/fetchNui';


export default function Home() {

    const [prompt, setPrompt] = useState({ show: false, title: '', description: '', description2: '' });

    useNuiEvent('createPrompt', (data) => {

        const regexColor = /~([^h])~([^~]+)/g;
        const regexBold = /~([h])~([^~]+)/g;
        const regexStop = /~s~/g;
        const regexLine = /\n/g;

        data.title = data.title.replace(regexColor, "<span class='$1'>$2</span>").replace(regexBold, "<span class='$1'>$2</span>").replace(regexStop, "").replace(regexLine, "<br />");
        data.description = data.description.replace(regexColor, "<span class='$1'>$2</span>").replace(regexBold, "<span class='$1'>$2</span>").replace(regexStop, "").replace(regexLine, "<br />");
        data.description2 = data.description2.replace(regexColor, "<span class='$1'>$2</span>").replace(regexBold, "<span class='$1'>$2</span>").replace(regexStop, "").replace(regexLine, "<br />");

        setPrompt(data)
    })



    useEffect(() => {
        if (prompt.show) {
            const keyHandler = (e: KeyboardEvent) => {
                if (["KeyY", "KeyN"].includes(e.code)) {
                    if (e.code === 'KeyY') {
                        fetchNui('prompt:accept')
                    } else {
                        fetchNui('prompt:refuse')
                    }
                }
            }

            window.addEventListener("keydown", keyHandler)

            return () => window.removeEventListener("keydown", keyHandler)
        }
    }, [prompt]);

    if (!prompt.show) return null;

    const handleAccept = () => {
        fetchNui('prompt:accept')
    }
    const handleDenied = () => {
        fetchNui('prompt:refuse')
    }



    return (
        <div className='prompt-container'>


            <div className='prompt-contain'>

                <h1 dangerouslySetInnerHTML={{ __html: prompt.title }}></h1>

                <div className='desc'>
                    <p dangerouslySetInnerHTML={{ __html: prompt.description }}></p>
                    <p dangerouslySetInnerHTML={{ __html: prompt.description2 }}></p>
                </div>

                <div className='buttons'>

                    <button className='refuse btn' onClick={handleDenied}>Refuser<b>(N)</b></button>
                    <button className='accept btn' onClick={handleAccept}>Accepter<b>(Y)</b></button>

                </div>

            </div>

        </div>
    )
}
