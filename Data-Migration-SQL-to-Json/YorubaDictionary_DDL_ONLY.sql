CREATE DATABASE  IF NOT EXISTS `yoruba_dictionary` /*!40100 DEFAULT CHARACTER SET utf8 COLLATE utf8_unicode_ci */;
USE `yoruba_dictionary`;
-- MySQL dump 10.13  Distrib 5.7.43, for Win64 (x86_64)
--
-- Host: ec2-34-216-203-18.us-west-2.compute.amazonaws.com    Database: yoruba_dictionary
-- ------------------------------------------------------
-- Server version	5.7.43

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `api_user`
--

DROP TABLE IF EXISTS `api_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `api_user` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `email` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `password` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `roles` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `username` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `UK_5x3eommwi436cv1hemmxd8twm` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `definition`
--

DROP TABLE IF EXISTS `definition`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `definition` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `content` longtext COLLATE utf8_unicode_ci NOT NULL,
  `english_translation` longtext COLLATE utf8_unicode_ci,
  `submitted_at` tinyblob,
  `word_id` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `FK_eb7i62ifgx1lj4qkgcxjhip8o` (`word_id`),
  CONSTRAINT `FK_eb7i62ifgx1lj4qkgcxjhip8o` FOREIGN KEY (`word_id`) REFERENCES `word_entry` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=627 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `definition_examples`
--

DROP TABLE IF EXISTS `definition_examples`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `definition_examples` (
  `definition_id` bigint(20) NOT NULL,
  `content` longtext COLLATE utf8_unicode_ci,
  `english_translation` longtext COLLATE utf8_unicode_ci,
  `type` int(11) DEFAULT NULL,
  KEY `FK_c3kcs4dn8wth4802dwknn0710` (`definition_id`),
  CONSTRAINT `FK_c3kcs4dn8wth4802dwknn0710` FOREIGN KEY (`definition_id`) REFERENCES `definition` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `diction`
--

DROP TABLE IF EXISTS `diction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `diction` (
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `audio_stream` longblob,
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `geo_location`
--

DROP TABLE IF EXISTS `geo_location`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `geo_location` (
  `place` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `region` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`place`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `word_entry`
--

DROP TABLE IF EXISTS `word_entry`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `word_entry` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `created_at` tinyblob,
  `famous_people` longtext COLLATE utf8_unicode_ci,
  `in_other_languages` longtext COLLATE utf8_unicode_ci,
  `ipa_notation` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `meaning` longtext COLLATE utf8_unicode_ci,
  `media` longtext COLLATE utf8_unicode_ci,
  `morphology` longtext COLLATE utf8_unicode_ci,
  `pronunciation` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `state` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `submitted_by` longtext COLLATE utf8_unicode_ci,
  `syllables` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `tags` longtext COLLATE utf8_unicode_ci,
  `tonal_mark` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `updated_at` tinyblob,
  `word` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `grammatical_feature` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `part_of_speech` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `style` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `UK_my23m4we2nsjsmy1j8hw7cfe3` (`word`)
) ENGINE=InnoDB AUTO_INCREMENT=403 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `word_entry_etymology`
--

DROP TABLE IF EXISTS `word_entry_etymology`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `word_entry_etymology` (
  `word_entry_id` bigint(20) NOT NULL,
  `meaning` longtext COLLATE utf8_unicode_ci,
  `part` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  KEY `FK_19k6cpb6pg1oh1ru2ntx5ecj0` (`word_entry_id`),
  CONSTRAINT `FK_19k6cpb6pg1oh1ru2ntx5ecj0` FOREIGN KEY (`word_entry_id`) REFERENCES `word_entry` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `word_entry_feedback`
--

DROP TABLE IF EXISTS `word_entry_feedback`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `word_entry_feedback` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `feedback` longtext COLLATE utf8_unicode_ci,
  `submitted_at` tinyblob,
  `word` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `word_entry_geo_location`
--

DROP TABLE IF EXISTS `word_entry_geo_location`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `word_entry_geo_location` (
  `word_entry_id` bigint(20) NOT NULL,
  `geo_location_place` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  KEY `FK_os5nn4c4uq70bsjb9kefban7y` (`geo_location_place`),
  CONSTRAINT `FK_os5nn4c4uq70bsjb9kefban7y` FOREIGN KEY (`geo_location_place`) REFERENCES `geo_location` (`place`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `word_entry_variants`
--

DROP TABLE IF EXISTS `word_entry_variants`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `word_entry_variants` (
  `word_entry_id` bigint(20) NOT NULL,
  `geo_location_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `word` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  KEY `FK_jjmms2h2dblhgdoosxr9a2dwo` (`geo_location_id`),
  KEY `FK_khnu0fbv4qwcvn0cov7ro9mhr` (`word_entry_id`),
  CONSTRAINT `FK_jjmms2h2dblhgdoosxr9a2dwo` FOREIGN KEY (`geo_location_id`) REFERENCES `geo_location` (`place`),
  CONSTRAINT `FK_khnu0fbv4qwcvn0cov7ro9mhr` FOREIGN KEY (`word_entry_id`) REFERENCES `word_entry` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `word_entry_media_links`
--

DROP TABLE IF EXISTS `word_entry_media_links`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `word_entry_media_links` (
  `word_entry_id` bigint(20) NOT NULL,
  `caption` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `link` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `type` int(11) DEFAULT NULL,
  KEY `FK_82d7c9gp1lm34lkotl0x7dixt` (`word_entry_id`),
  CONSTRAINT `FK_82d7c9gp1lm34lkotl0x7dixt` FOREIGN KEY (`word_entry_id`) REFERENCES `word_entry` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
