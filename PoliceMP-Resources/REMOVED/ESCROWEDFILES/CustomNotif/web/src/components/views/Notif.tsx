import React, { useEffect, useState, useRef } from 'react';
import './Notif.scss';
import './Regles.scss';

import { useNuiEvent } from '../../hooks/useNuiEvent';


export interface Notification {
    type: string;
    content: string;
    time: number;
    startTime: number;
    widthPercentage: string;
    percentage: string;
    visible: boolean;
    indelete: number;
    animation: string;
    image: string;
    icontop: string;
    texttop: string;
    iconmid: string;
    textmid: string;
    iconbottom: string;
    textbottom: string;
    colorbadge: string;
    textbadge: string;
    footerbadgecolor: string;
    footerbadgetext: string;
}

const Notif = () => {


    const [notif, setNotif] = useState<Notification[]>(
        localStorage.getItem("notif")
            ? JSON.parse(localStorage.getItem("notif")!)
            : []
    );

    useNuiEvent("SendNotif:Classic", (data: Notification) => {
        data.startTime = Date.now();
        data.indelete = 100;
        data.type = "1";
        data.widthPercentage = '100%';
        setNotif((prevNotif) => [...prevNotif, data]);
    });

    useNuiEvent("SendNotif:Big", (data: Notification) => {
        data.startTime = Date.now();
        data.indelete = 100;
        data.type = "2";
        data.widthPercentage = '100%';
        setNotif((prevNotif) => [...prevNotif, data]);
    });


    useEffect(() => {
        let animationFrameId: number;

        const animationFrame = () => {
            const currentTime = Date.now();

            setNotif((prevNotif) => {
                const updatedNotif = prevNotif.map((notification) => {
                    if (notification.widthPercentage == "100%") {
                        notification.visible = true;
                    }
                    const elapsedTime =
                        (currentTime - notification.startTime) / 1000;
                    const progress =
                        notification.time !== undefined
                            ? (elapsedTime * 2) / notification.time
                            : 1;
                    const clampedProgress = Math.max(Math.min(progress, 1), 0);
                    const widthPercentage =
                        clampedProgress !== 0
                            ? ((1 - clampedProgress) * 100).toFixed(2) + "%"
                            : "0%";
                    if (notification.widthPercentage == "0.00%") {
                        notification.visible = false;
                    }
                    if (notification.widthPercentage == "0.00%" && notification.visible == false && notification.indelete > 0) {
                        notification.indelete = notification.indelete - 1;
                    }
                    if (notification.startTime + 1000 > currentTime) {
                        notification.animation = 'visible enter-from-left'
                    } else {
                        notification.animation = ''
                    }
                    return {
                        ...notification,
                        widthPercentage,
                    };

                });
                return updatedNotif;
            });

            animationFrameId = requestAnimationFrame(animationFrame);
        };


        animationFrameId = requestAnimationFrame(animationFrame);

        return () => {
            cancelAnimationFrame(animationFrameId);
        };
    }, [notif]);

    useEffect(() => {
        if (notif.length === 0 || notif.length === 1) {
            return;
        }
        const allNotificationsHaveSameTime = notif.every(
            (notification) => notification.indelete === 0 && notification.visible === notif[0].visible
        );

        if (allNotificationsHaveSameTime) {
            setNotif([]);
        }



    }, [notif]);


    useEffect(() => {
        localStorage.setItem("notif", JSON.stringify(notif));
    }, [notif]);

    return (
        <div className='notif-container'>

            <div className='notif-contain'>

                {notif.map((notification, index) => {
                    if (notification.type == "1" && notification.image != 'false') {
                        return (
                            <div className={`notifone notif ${notification.visible ? notification.animation : 'exit-to-left'}`}>
                                <div className='img-border'>
                                    <img src={notification.image} alt='' />
                                </div>
                                <div dangerouslySetInnerHTML={{ __html: notification.content }}></div>
                                <div className='progress-bar'>
                                    <div className='progress-bar-fill' style={{ width: notification.widthPercentage }}></div>
                                </div>
                            </div>
                        );
                    } else if (notification.type == "1") {
                        return (
                            <div className={`notifone notif ${notification.visible ? notification.animation : 'exit-to-left'}`}>
                                <div dangerouslySetInnerHTML={{ __html: notification.content }}></div>
                                <div className='progress-bar'>
                                    <div className='progress-bar-fill' style={{ width: notification.widthPercentage }}></div>
                                </div>
                            </div>
                        )
                    } else if (notification.type == "2" && notification.footerbadgetext != 'false') {
                        return (
                            <div className={`notiftwo notif ${notification.visible ? notification.animation : 'exit-to-left'}`}>

                                <div className='header'>
                                    <div className='left'>
                                        <img src={notification.icontop} alt="" />
                                        <div dangerouslySetInnerHTML={{ __html: notification.texttop }}></div>
                                    </div>
                                    <div className='right'><p style={{ background: notification.colorbadge }}>{notification.textbadge}</p></div>
                                </div>

                                <div className='footer'>

                                    <div className='left'>
                                        <div className='desc'>
                                            <img src={notification.iconmid} alt="" />
                                            <div dangerouslySetInnerHTML={{ __html: notification.textmid }}></div>
                                        </div>

                                        <div className='desc'>
                                            <img src={notification.iconbottom} alt="" />
                                            <div dangerouslySetInnerHTML={{ __html: notification.textbottom }}></div>
                                        </div>
                                    </div>

                                    <div className='right'><p style={{ background: notification.footerbadgecolor }}>{notification.footerbadgetext}</p></div>


                                </div>


                                <div className='progress-bar'>
                                    <div className='progress-bar-fill' style={{ width: notification.widthPercentage }}></div>
                                </div>
                            </div>
                        )
                    } else if (notification.type == "2" && notification.footerbadgetext == 'false') {
                        return (
                            <div className={`notiftwo notif ${notification.visible ? notification.animation : 'exit-to-left'}`}>

                                <div className='header'>
                                    <div className='left'>
                                        <img src={notification.icontop} alt="" />
                                        <div dangerouslySetInnerHTML={{ __html: notification.texttop }}></div>
                                    </div>
                                    <div className='right'><p style={{ background: notification.colorbadge }}>{notification.textbadge}</p></div>
                                </div>

                                <div className='footer'>

                                    <div className='left'>
                                        <div className='desc'>
                                            <img src={notification.iconmid} alt="" />
                                            <div dangerouslySetInnerHTML={{ __html: notification.textmid }}></div>
                                        </div>

                                        <div className='desc'>
                                            <img src={notification.iconbottom} alt="" />
                                            <div dangerouslySetInnerHTML={{ __html: notification.textbottom }}></div>
                                        </div>
                                    </div>


                                </div>


                                <div className='progress-bar'>
                                    <div className='progress-bar-fill' style={{ width: notification.widthPercentage }}></div>
                                </div>
                            </div>
                        )
                    }

                })}

            </div >

        </div >

    );

};

export default Notif;