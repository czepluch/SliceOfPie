# ************************************************************
# Sequel Pro SQL dump
# Version 3408
#
# http://www.sequelpro.com/
# http://code.google.com/p/sequel-pro/
#
# Host: 79.143.187.42 (MySQL 5.5.28-0ubuntu0.12.04.2)
# Database: sliceofpie
# Generation Time: 2012-12-16 15:15:26 +0000
# ************************************************************


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


# Dump of table document
# ------------------------------------------------------------

DROP TABLE IF EXISTS `document`;

CREATE TABLE `document` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `projectId` int(10) DEFAULT NULL,
  `folderId` int(10) DEFAULT NULL,
  `title` varchar(100) DEFAULT NULL,
  `currentRevision` text,
  `currentHash` int(32) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `d_projectId` (`projectId`),
  KEY `d_folderId` (`folderId`),
  CONSTRAINT `d_folderId` FOREIGN KEY (`folderId`) REFERENCES `folder` (`id`),
  CONSTRAINT `d_projectId` FOREIGN KEY (`projectId`) REFERENCES `project` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;



# Dump of table folder
# ------------------------------------------------------------

DROP TABLE IF EXISTS `folder`;

CREATE TABLE `folder` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `projectId` int(10) DEFAULT NULL,
  `folderId` int(10) DEFAULT NULL,
  `title` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `f_projectId` (`projectId`),
  KEY `f_folderId` (`folderId`),
  CONSTRAINT `f_folderId` FOREIGN KEY (`folderId`) REFERENCES `folder` (`id`),
  CONSTRAINT `f_projectId` FOREIGN KEY (`projectId`) REFERENCES `project` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;



# Dump of table project
# ------------------------------------------------------------

DROP TABLE IF EXISTS `project`;

CREATE TABLE `project` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `title` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;



# Dump of table project_users
# ------------------------------------------------------------

DROP TABLE IF EXISTS `project_users`;

CREATE TABLE `project_users` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `projectId` int(10) DEFAULT NULL,
  `userEmail` varchar(100) DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `projectId` (`projectId`,`userEmail`),
  KEY `pu_projectId` (`projectId`),
  KEY `pu_userId` (`userEmail`),
  CONSTRAINT `pu_projectId` FOREIGN KEY (`projectId`) REFERENCES `project` (`id`),
  CONSTRAINT `pu_userEmail` FOREIGN KEY (`userEmail`) REFERENCES `user` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;



# Dump of table revision
# ------------------------------------------------------------

DROP TABLE IF EXISTS `revision`;

CREATE TABLE `revision` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `documentId` int(10) DEFAULT NULL,
  `content` longtext,
  `contentHash` int(32) DEFAULT NULL,
  `timestamp` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `r_documentId` (`documentId`),
  CONSTRAINT `r_documentId` FOREIGN KEY (`documentId`) REFERENCES `document` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;



# Dump of table user
# ------------------------------------------------------------

DROP TABLE IF EXISTS `user`;

CREATE TABLE `user` (
  `email` varchar(100) NOT NULL DEFAULT '',
  `password` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;




/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
