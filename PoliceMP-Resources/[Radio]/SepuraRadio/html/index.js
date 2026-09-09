let Power = false;
let TalkGroups;
let SelectedScreen;
let TempSelectedFolder;
let SelectedFolder;
let SelectedTalkGroup;
let Contacts = [];
let isOnCall = false;
let isRequestingCall = false;
let Config = {}
let ResourceName = 'SepuraRadio'

let Screens = {
    ["Off"]:            { Type: "Custom", Build: () => Build('Off') },
    ["Home"]:           { Type: "Custom", Build: () => Build('Home') },
    ["Menu"]:           { Type: "Custom", Build: () => Build('Menu') },
    ["Speaking"]:       { Type: "Custom", Build: () => Build('Speaking') },
    ["Call"]:           { Type: "Custom", Build: () => Build('Call') },
    ['Folders']:        { Type: 'List',   Build: () => List('Folders', Object.keys(TalkGroups), 'fa-folder') },
    ['TalkGroups']:     { Type: 'List',   Build: () => List('TalkGroups', TalkGroups[TempSelectedFolder], 'fa-users') },
    ['Contacts']:       { Type: 'List',   Build: () => List('Contacts', Contacts, 'fa-user') },
    ['Settings']:       { Type: 'List',   Build: () => List('Settings', Settings, 'fa-cog') },
    ['Radio Volume']:   { Type: "Custom", Build: () => Build('Radio Volume') },
    ['Radio Position']: { Type: "Custom", Build: () => Build('Radio Position') },
    ['Radio Scale']:    { Type: "Custom", Build: () => Build('Radio Scale') },
};

function SelectListItem(pValue, pAction){
    
    if(pAction === 'TalkGroups') TempSelectedFolder = pValue.slice(1);

    if(pAction === 'FinishTG'){
        SelectedFolder = TempSelectedFolder;
        SelectedTalkGroup = pValue.startsWith(' ') ? pValue.slice(1) : pValue;
        $.post('http://' + ResourceName + '/SelectTalkGroup', JSON.stringify({ SelectedTalkGroup: SelectedTalkGroup, SelectedFolder: TempSelectedFolder }));
        Screens['Home'].Build();
    }
    if(pAction === 'RequestCall'){
        isRequestingCall = true
        $.post('http://' + ResourceName + '/RequestCall', JSON.stringify({ UserId: Contacts[pValue.startsWith(' ') ? pValue.slice(1) : pValue] }));
    }
    
    if(pValue.includes('Show Speaker')) return SelectSettings('Show Speaker')

    if(Screens[pAction]) Screens[pAction].Build();
}

function Build(pPage){
    HideAll();

    if(pPage === 'Off'){
        $('#Radio-Off').show();
        $('#Screen').hide();
    }
    else if(pPage === 'Home'){
        $('#SelectedFolder').text(SelectedFolder);
        $('#SelectedChannel').text(SelectedTalkGroup);
    }

    else if(pPage === 'Radio Volume' || pPage === 'Radio Position' || pPage === 'Radio Scale') SelectSettings(pPage);

    CreateDisplayButtons(pPage);
    $(`#${pPage}`).show();
    if(pPage !== 'Speaking') SelectedScreen = pPage;
}

function List(pPage, pItems, pIcon){
    HideAll();

    if(pPage == 'Contacts') pItems = Object.keys(pItems);
    let html = pItems.map((item, index) => `<h1 class="fw-light ps-1 ${index === 0 ? 'Selected' : ''}" id="list-items-${index}"><i class="fas ${pIcon}"></i> ${item}${item === 'Show Speaker' ? `: ${ShowSpeaker ? 'Yes' : 'No'}` : ''}</h1>`)

    $(`.List-Items`).html(html);
    $(`#List-Title`).text(pPage !== 'TalkGroups' ? `1: ${pPage}` : `1: ${TempSelectedFolder}`)

    CreateDisplayButtons(pPage);
    $(`#List`).show();
    SelectedScreen = pPage;
}

function HideAll(){
    $('.Radio-Off').hide();
    $('#Home').hide();
    $('#List').hide();
    $('#Menu').hide();
    $('#Speaking').hide();
    $('#Settings').hide();
    $('#Call').hide();
}

window.addEventListener('message', function(pEvent) {
    const data = pEvent.data;
    
    if(data.Action === 'ViewRadio') $('body').show();
    
    if(data.Action === 'AutoSelectTG'){
        if(SelectedFolder == null || SelectedTalkGroup == null) return;
        for(let i=0; i <= TalkGroups[SelectedFolder].length; i++){
            if(TalkGroups[SelectedFolder][i] == SelectedTalkGroup){
                if(i+2 <= TalkGroups[SelectedFolder].length) SelectListItem(TalkGroups[SelectedFolder][i+1], 'FinishTG')
                else SelectListItem(TalkGroups[SelectedFolder][0], 'FinishTG');
                PlayAudio('SingleBleep')
                break;
            };
        }
    }

    if(data.Action === 'AutoSelectFolder'){
        for(let i=0; i < data.ATG.length; i++){

            if(data.ATG[i] == SelectedFolder) {
                if(i+2 <= data.ATG.length) TempSelectedFolder = data.ATG[i+1]
                else TempSelectedFolder = data.ATG[0];

                SelectListItem(TalkGroups[TempSelectedFolder][0], 'FinishTG');
                PlayAudio('SingleBleep')
                break;
            }
        }
    }

    if(data.Config){
        TalkGroups = data.Config.Frequencies;
        Config = data.Config;
        ShowSpeaker = data.Config.DefaultShowSpeaker;
        ResourceName = data.ResourceName || 'SepuraRadio'
    }
    
    if(data.Contacts) Contacts = data.Contacts;

    if(data.Speaking){
        if(!Power || !ShowSpeaker) return;
        if(!data.IsSpeaking){
            Screens[SelectedScreen].Build();
            $('body').hide();
            return;
        }

        if(SelectedTalkGroup !== data.TalkGroupName) return;

        $('body').show();
        $('#Speaking-User-Name').text(data.UserSpeaking.toUpperCase());
        $('#Speaking-User-TalkGroup').text(data.TalkGroupName);
        $('#Speaking-User-Folder').text(data.FolderName);
        Screens['Speaking'].Build();
    }

    if(data.Action === 'Call-InBound'){
        isOnCall = true
        PlayAudio('P2P', true);
        $('body').show();
        $('#Call-User-Name').text(data.PlayerName || 'UNKOWN');
        Screens['Call'].Build();
    }

    if(data.Action === 'EndCall'){
        SelectPhoneReject(true);
    }

    if(data.Action === 'Audio'){
        PlayAudio(data.Audio)
    }

})

Screens['Off'].Build();
$('body').hide();