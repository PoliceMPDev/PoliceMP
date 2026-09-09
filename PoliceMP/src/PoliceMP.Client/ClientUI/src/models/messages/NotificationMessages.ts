export interface NotificationMessageContent 
{
    Label: string;
    Content: string;
}

export interface NotificationMessage
{
    Title: string;
    Type: string;
    Message?: string;
    MultiMessage?: NotificationMessageContent[];
    AutoClose: number | null;
    ImageUrl: string | null;
}

