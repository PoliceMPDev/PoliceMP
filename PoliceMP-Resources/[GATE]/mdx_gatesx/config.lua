Config = {}

Config.Gates = {
    ['Alperton Police Station'] = {
        {
            GateCoords = vector3(-1059.99536, -2034.64368, 12.249671),
            GateRotation = vector3(0.0, 0.0, -45.0),
            KeyCoordsOutside = vector3(-1064.41, -2033.49, 13.27),
            KeyCoordsInside = vector3(-1053.14, -2038.67, 12.73),
            InteractionDistance = 15.0, -- Must be defined
            model = 'mdx_alp_bifolding',
            animationDict = 'mdx@mdx_alp_bifold',
            animationName = 'mdx_alp_bifold',
        }
    },
    ['NPAS Base'] = {
        {
            GateCoords = vector3(-709.548157, -1383.15027, 6.38),
            GateRotation = vector3(0.0, 0.0, -40.0),
            KeyCoordsOutside = vector3(-710.86, -1374.92, 5.27),
            KeyCoordsInside = vector3(-711.73, -1387.24, 5),
            InteractionDistance = 15.0, -- Must be defined
            model = 'mdx_npas_gates',
            animationDict = 'clip@mdx_npas_gates',
            animationName = 'mdx_npas_gates',
        }
    },
    ['ARV Base'] = {
        {
            GateCoords = vector3(-313.320343, -2680.46436, 7.50234747),
            GateRotation = vector3(0.0, 0.0, 45.0),
            KeyCoordsOutside = vector3(-313.417, -2671.588, 5.02833176),
            KeyCoordsInside = vector3(-302.367279, -2684.59888, 5.02833176),
            InteractionDistance = 15.0, -- Must be defined
            model = 'mdxarv_gates',
            animationDict = 'clip@mdxarv_gates',
            animationName = 'mdxarv_gates',
        }
    }
}