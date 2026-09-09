fx_version "cerulean"

description "Prompt"
author "Sertinox & CyteUI"
version '1.0.0'

lua54 'yes'

games {
  "gta5",
}

ui_page 'web/build/index.html'

client_script "client/**/*"
escrow_ignore "client/**/*"

files {
  'web/build/index.html',
  'web/build/**/*',
}

dependency '/assetpacks'