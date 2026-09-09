const PageButtons = { 
    ['Home']: [ 
        { label: 'Folders',  onSelect: () => Screens['Folders'].Build() }, 
        { label: 'Menu',     onSelect: () => Screens['Menu'].Build() }, 
        { label: 'Contacts', onSelect: () => Screens['Contacts'].Build() } 
    ],
    ['Menu']: [ 
        { label: 'Select', onSelect: (pValue) => SelectMenuItem(pValue) }, 
        { label: 'Cancel', onSelect: () => Screens['Home'].Build() } 
    ],
    ['Folders']: [ 
        { label: 'Select', onSelect: (pValue) => SelectListItem(pValue, 'TalkGroups') }, 
        { label: 'Cancel', onSelect: () => Screens['Home'].Build() } 
    ],
    ['TalkGroups']: [ 
        { label: 'Select', onSelect: (pValue) => SelectListItem(pValue, 'FinishTG') }, 
        { label: 'Cancel', onSelect: () => Screens['Folders'].Build() } 
    ],
    ['Speaking']: [ 
        { label: 'Clear',     onSelect: () => Screens['Home'].Build() },
        { label: 'Settings',  onSelect: () => Screens['Settings'].Build() } 
    ],
    ['Call']: [ 
        { label: 'Clear',  onSelect: () => SelectPhoneReject() }
    ],
    ['Contacts']: [ 
        { label: 'Call',   onSelect: (pValue) => SelectListItem(pValue, 'RequestCall') }, 
        { label: 'Cancel', onSelect: () => Screens['Home'].Build() } 
    ],
    ['Settings']: [ 
        { label: 'Select', onSelect: (pValue) => SelectListItem(pValue, pValue.slice(1)) }, 
        { label: 'Cancel', onSelect: () => Screens['Home'].Build() } 
    ],
    ['Radio Volume']: [ 
        { label: 'Menu', onSelect: () => Screens['Menu'].Build() },
        { label: 'Back', onSelect: () => Screens['Settings'].Build() }
    ],
    ['Radio Position']: [ 
        { label: 'Menu', onSelect: () => Screens['Menu'].Build() },
        { label: 'Back', onSelect: () => Screens['Settings'].Build() }
    ],
    ['Radio Scale']: [ 
        { label: 'Menu', onSelect: () => Screens['Menu'].Build() },
        { label: 'Back', onSelect: () => Screens['Settings'].Build() }
    ],
}

const MenuSelectButtonKey = { ['left']: 0, ['middle']: 1, ['right']: 2 }