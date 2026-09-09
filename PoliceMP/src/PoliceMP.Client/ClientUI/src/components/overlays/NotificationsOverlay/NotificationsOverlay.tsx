import * as React from 'react'
import {useState} from 'react'
import styled from 'styled-components'
import {useOverlayEvent} from '../../../hooks/useOverlayEvent'
import {useOverlayId} from '../../../hooks/useOverlayId'
import {toast, ToastContainer} from 'react-toastify';
import "react-toastify/dist/ReactToastify.css";
import {NotificationMessage} from '../../../models/messages/NotificationMessages'

const StyledHud = styled.div`
    display: block;
    margin: 0;
    padding: 5px;

    .notification-wrapper {
        top: 30% !important;
    }

    .panic-toast {
        background-color: red !important;
        color: white !important;
     }
     
     .panic-toast .Toastify__progress-bar {
        background: white !important;
    }

    .text-underline{
        text-decoration: underline;
    }
    
    .Toastify__toast-icon {
        width: auto;
    }
    .Toastify__toast-icon > svg {
        width: 20px;
    }
    .Toastify__toast-icon > img {
        width: 65px;
    }
`

export interface NotificationsOverlayProps {
}

const NotificationsOverlay = (props: NotificationsOverlayProps) => {
    const overlayId = useOverlayId();
    const [notificationDetails, setNotificationDetails] = useState<NotificationMessage>([]);
    const [interactionName, setInteractionName] = useState("");

    const displayToast = (type: string, title: string, message: string|JSX.Element[], autoClose: number | null, imageUrl: string | null) => {
        var toastContent;
        if (typeof message == 'object') {
            toastContent = (
                <>
                    <strong>{title}</strong>
                    <div>{message}</div>
                </>
            );
        } else {
            toastContent = (
                <>
                    <strong>{title}</strong>
                    <div dangerouslySetInnerHTML={{__html: message}}/>
                </>
            );
        }
        let data = {};
        if (null !== autoClose) {
            data.autoClose = autoClose * 1000;
        }
        if (null !== imageUrl) {
            data.icon = ({theme, type}) => <img src={imageUrl}/>;
        }
        
        switch (type) {
            case 'info':
                toast.info(toastContent, data);
                break;
            case 'error':
                toast.error(toastContent, data);
                break;
            case 'success':
                toast.success(toastContent, data);
                break;
            case 'warn':
                toast.warn(toastContent, data);
                break;
            case 'dark':
                toast.dark(toastContent, data);
                break;
            case 'panic':
                toast(toastContent, {
                    className: "panic-toast" // Apply the custom toast style
                });
                break;
            default:
                toast(toastContent, data); // Default fallback
        }
    }

    const handleSingleMessageNotification = (title: string, type: string, message: string, autoClose: number | null, imageUrl: string | null) => {
        displayToast(type, title, message, autoClose, imageUrl);
    }

    const handleMultiMessageNotification = (title: string, type: string, messages: NotificationMessageContent[], autoClose: number | null, imageUrl: string | null) => {
        const combinedMessage = messages.map(m => <div><strong>{m.Label}:</strong> {m.Content}</div>);
        displayToast(type, title, combinedMessage, autoClose, imageUrl);
    }

    useOverlayEvent<NotificationMessage>(overlayId, "SendNotification", target => {
        // Check if Message or MultiMessage is populated and call the appropriate method.
        if (target.Message) {
            handleSingleMessageNotification(target.Title, target.Type, target.Message, target.AutoClose, target.ImageUrl);
        } else if (target.MultiMessage && target.MultiMessage.length > 0) {
            handleMultiMessageNotification(target.Title, target.Type, target.MultiMessage, target.AutoClose, target.ImageUrl);
        }
    });

    return <StyledHud>
          <ToastContainer
            position="top-left"
            closeButton={false}
            autoClose={8000}
            className="notification-wrapper"
            theme="dark">
                </ToastContainer>
    </StyledHud>
}

export default NotificationsOverlay
