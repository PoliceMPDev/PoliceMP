-- For exemple if you want to send a notification from an another resource, you can do this:


-- Put that in this script :

/*
RegisterNetEvent('SendNotif:Classic')
AddEventHandler('SendNotif:Classic', function(content, time, image)
    SendReactMessage('SendNotif:Classic', { content = content, time = time, image = image })
end)
*/

-- And put that in the script where you want to send the notification :
/*
TriggerEvent('SendNotif:Classic', 'Sertinox <span style="color: #ff0000;">x</span> CyteUi', 15, 'https://cdn.discordapp.com/attachments/1123497436812423198/1138918005376548894/7600_7_06-modified.png')
*/



RegisterCommand('TestBigNotif', function()
    SendReactMessage('SendNotif:Big', { 
    content = "Sertinox <span style='color: #ff0000;'>x</span> CyteUi",
    time = 15,
    icontop = 'https://cdn.discordapp.com/attachments/1123497436812423198/1138918005376548894/7600_7_06-modified.png',
    texttop = 'Sertinox <span style="color: #ff0000;">x</span> CyteUi',
    iconmid = 'https://cdn.discordapp.com/attachments/1123497436812423198/1138918005376548894/7600_7_06-modified.png',
    textmid = 'Sertinox <span style="color: #ff0000;">x</span> CyteUi',
    iconbottom = 'https://cdn.discordapp.com/attachments/1123497436812423198/1138918005376548894/7600_7_06-modified.png',
    textbottom = 'Sertinox <span style="color: #ff0000;">x</span> CyteUi',
    colorbadge = '#ff0000',
    textbadge = 'GOOD',
    footerbadgecolor = '#ffffffb3',
    footerbadgetext = 'GOOD',
    })
    -- If you want to send the same notification without the image, you can do this:
    -- SendReactMessage('SendNotif:Big', {  content = content, time = 15, texttop = 'Sertinox <span style="color: #ff0000;">x</span> CyteUi', textmid = 'Sertinox <span style="color: #ff0000;">x</span> CyteUi', textbottom = 'Sertinox <span style="color: #ff0000;">x</span> CyteUi', colorbadge = '#ff0000', textbadge = 'GOOD', footerbadgecolor = '#ffffffb3', footerbadgetext = 'GOOD' })
end)

RegisterCommand('TestClassic', function()
    SendReactMessage('SendNotif:Classic', { 
        content = "Sertinox <span style='color: #ff0000;'>x</span> CyteUi",
        time = 15,
        image = 'https://cdn.discordapp.com/attachments/1123497436812423198/1138918005376548894/7600_7_06-modified.png',
    })
    -- If you want to send the same notification without the image, you can do this: 
    -- SendReactMessage('SendNotif:Classic', {  content = content, time = 15, image = 'false' })
    -- or 
    -- SendReactMessage('SendNotif:Classic', {  content = content, time = 15 })
end)