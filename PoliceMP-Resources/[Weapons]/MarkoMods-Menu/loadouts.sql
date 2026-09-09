CREATE TABLE `markomods_loadouts` (
  
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `license` varchar(255) DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  `weapons` varchar(255) DEFAULT NULL,
  `components` longtext,

  PRIMARY KEY (`id`)
);