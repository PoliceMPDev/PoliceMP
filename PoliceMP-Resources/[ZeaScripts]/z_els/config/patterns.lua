--@param: Please refer to our resource documentation for assistance with configuring this resource: docs.zeadevelopment.com.

-- THIS IS FOR ELS VEHICLES ..

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------

---@class patterns : Configuration
patterns = {}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@type Primary Lighting

patterns['primary'] = {
   -- Table Start --
   ['Double-Flash-Fast'] = {
      { activate = {1}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {1}, delay = 100 },
      { activate = {4}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {4}, delay = 100 },
   },
   -- Table End --
   -- Table Start --
   ['Triple-Flash-Fast'] = {
      { activate = {1, 5, 3}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {1, 5, 3}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {1, 5, 3}, delay = 100 },
      { activate = {2, 6, 4}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {2, 6, 4}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {2, 6, 4}, delay = 100 },
   },
   -- Table End --
   -- Table Start --
   ['Triple-Flash-Slow'] = {
      { activate = {1}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {1}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {1}, delay = 250 },
      { activate = {4}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {4}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {4}, delay = 250 },
   },
   -- Table End --
   -- Table Start --
   ['Quadrouple-Flash-Fast'] = {
      { activate = {}, delay = 20 },
      { activate = {1, 3, 5}, delay = 40 },
      { activate = {}, delay = 20 },
      { activate = {1, 3, 5}, delay = 40 },
      { activate = {}, delay = 20 },
      { activate = {1, 3, 5}, delay = 40 },
      { activate = {}, delay = 20 },
      { activate = {1, 3, 5}, delay = 40 },
      { activate = {}, delay = 20 },
      { activate = {2, 4, 6}, delay = 40 },
      { activate = {}, delay = 20 },
      { activate = {2, 4, 6}, delay = 40 },
      { activate = {}, delay = 20 },
      { activate = {2, 4, 6}, delay = 40 },
      { activate = {}, delay = 20 },
      { activate = {2, 4, 6}, delay = 40 },
   },
   -- Table End --
}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@type On Scene Lighting

patterns['onscene'] = {
   -- Table Start --
   ['Double-Flash-Fast'] = {
      { activate = {1}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {1}, delay = 100 },
      { activate = {4}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {4}, delay = 100 },
   },
   -- Table End --
   -- Table Start --
   ['Quadrouple-Flash-Slow'] = {
      { activate = {3, 9}, delay = 320 },
      { activate = {}, delay = 65 },
      { activate = {4, 7}, delay = 320 },
      { activate = {}, delay = 65 },
      { activate = {3, 9}, delay = 320 },
      { activate = {}, delay = 65 },
      { activate = {4, 7}, delay = 320 },
      { activate = {3, 9}, delay = 320 },
      { activate = {}, delay = 65 },
      { activate = {4, 7}, delay = 320 },
      { activate = {}, delay = 65 },
      { activate = {3, 9}, delay = 320 },
      { activate = {}, delay = 65 },
      { activate = {4, 7}, delay = 320 },
   },
   -- Table End --
}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@type Front Whites Lighting

patterns['front_whites'] = {
   -- Table Start --
   ['Single-Flash-Slow'] = {
      { activate = {5}, delay = 350 },
      { activate = {}, delay = 25 },
      { activate = {6}, delay = 350 },
      { activate = {}, delay = 25 },
   },
   -- Table End --
   -- Table Start --
   ['Full-Flash-Slow'] = {
      { activate = {5, 6}, delay = 75 },
      { activate = {}, delay = 250 },
      { activate = {5, 6}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {5, 6}, delay = 250 },
   },
   -- Table End --
}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@type Front Blues Lighting

patterns['front_blues'] = {
   -- Table Start --
   ['Full-Flash-Slow'] = {
      { activate = {2, 3}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {2, 3}, delay = 75 },
      { activate = {}, delay = 250 },
      { activate = {2, 3}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {2, 3}, delay = 250 },
   },
   -- Table End --
   -- Table Start --
   ['Single-Flash-Slow'] = {
      { activate = {}, delay = 250 },
      { activate = {3}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {3}, delay = 75 },
      { activate = {}, delay = 25 },
   },
      ['JBC_FrontBlue'] = {
      { activate = {1, 6}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {1, 6}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {1, 6}, delay = 100 },
      { activate = {2, 5}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {2, 5}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {2, 5}, delay = 100 },
   },
   -- Table End --
}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@type Rear Reds Lighting

patterns['rear_reds'] = {
   -- Table Start --
   ['Singe-Flash-Slow'] = {
      { activate = {9}, delay = 75 },
      { activate = {}, delay = 250 },
      { activate = {7}, delay = 75 },
      { activate = {}, delay = 25 },
      { activate = {9}, delay = 250 },
   },
   -- Table End --
   -- Table Start --
   ['Double-Flash-Slow'] = {
      { activate = {}, delay = 25 },
      { activate = {9}, delay = 350 },
      { activate = {}, delay = 100 },
      { activate = {9}, delay = 350 },
      { activate = {7}, delay = 350 },
      { activate = {}, delay = 100 },
      { activate = {7}, delay = 350 },
      { activate = {}, delay = 25 },
   },
   -- Table End --
   -- Table Start --
   ['Full-Flash-Slow'] = {
      { activate = {7}, delay = 125 },
      { activate = {}, delay = 250 },
      { activate = {9}, delay = 125 },
      { activate = {}, delay = 250 },
      { activate = {7}, delay = 250 },
   },
   -- Table End --
}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@type Rear Blues Lighting

patterns['rear_blues'] = {
   -- Table Start --
   ['Full-Flash-Slow'] = {
      { activate = {4}, delay = 125 },
      { activate = {}, delay = 250 },
      { activate = {3}, delay = 125 },
      { activate = {}, delay = 250 },
      { activate = {4}, delay = 250 },
   },
   -- Table End --
}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@type Message Board Lighting

patterns['message_board'] = {
   -- Table Start --
   ['Cycle-Slow'] = {
      { activate = {5}, delay = 500 },
      { activate = {}, delay = 25 },
      { activate = {6}, delay = 500 },
      { activate = {}, delay = 25 },
   },
   -- Table End --
   -- Table Start --
   ['Flash-Single'] = {
      { activate = {5}, delay = 500 },
      { activate = {}, delay = 250 },
   },
   -- Table End --
}