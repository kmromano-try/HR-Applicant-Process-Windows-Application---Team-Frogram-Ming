CREATE DATABASE  IF NOT EXISTS `hr_applicant_system` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `hr_applicant_system`;
-- MySQL dump 10.13  Distrib 8.0.46, for macos15 (arm64)
--
-- Host: localhost    Database: hr_applicant_system
-- ------------------------------------------------------
-- Server version	9.7.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
SET @MYSQLDUMP_TEMP_LOG_BIN = @@SESSION.SQL_LOG_BIN;
SET @@SESSION.SQL_LOG_BIN= 0;

--
-- GTID state at the beginning of the backup 
--

SET @@GLOBAL.GTID_PURGED=/*!80000 '+'*/ '6adb0c50-5f1f-11f1-8357-c74b25d7c75f:1-102';

--
-- Table structure for table `Applicants`
--

DROP TABLE IF EXISTS `Applicants`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Applicants` (
  `ApplicantID` int NOT NULL AUTO_INCREMENT,
  `FullName` varchar(255) DEFAULT NULL,
  `Email` varchar(255) NOT NULL,
  `ContactNumber` varchar(50) DEFAULT NULL,
  `Password` varchar(255) DEFAULT NULL,
  `Birthdate` date DEFAULT NULL,
  `Bio` text,
  `Experience` text,
  `ResumeFilePath` varchar(512) DEFAULT NULL,
  PRIMARY KEY (`ApplicantID`),
  UNIQUE KEY `Email` (`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Applicants`
--

LOCK TABLES `Applicants` WRITE;
/*!40000 ALTER TABLE `Applicants` DISABLE KEYS */;
INSERT INTO `Applicants` VALUES (2,'turt ramona','iunnobro@gmail.com','911',NULL,NULL,'Im dyin bro',NULL,NULL),(3,'New Applicant','test@gmail.com','119',NULL,NULL,'test',NULL,NULL),(4,'DIO','ZAWARUDO@gmail.com','0999',NULL,NULL,NULL,NULL,NULL),(5,'Speed','ineedthis@gmail.com','00',NULL,NULL,NULL,NULL,NULL),(6,'test2','testtest@gmail.com','00',NULL,NULL,NULL,NULL,NULL),(7,'test2','test2@gmail.com','00',NULL,NULL,'I',NULL,NULL),(8,'test3','test3@gmail.com','00',NULL,NULL,NULL,NULL,NULL),(9,'test4','test4@gmail.com','00',NULL,NULL,'',NULL,NULL),(10,'Jhazmine Evasco','jhaz@gmail.com','999',NULL,NULL,'I have very good yes',NULL,NULL),(11,'Albret','albertday@gmail.com','67',NULL,NULL,NULL,NULL,NULL),(12,'Zeki Li iT','liit@gmail.com','67',NULL,NULL,NULL,NULL,NULL),(13,'test5','test5@gmail.com','911',NULL,NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `Applicants` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Applications`
--

DROP TABLE IF EXISTS `Applications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Applications` (
  `ApplicationID` int NOT NULL AUTO_INCREMENT,
  `ApplicantID` int NOT NULL,
  `VacancyID` int NOT NULL,
  `Status` varchar(100) DEFAULT 'Pending',
  `DateApplied` date DEFAULT (curdate()),
  `StaffFeedback` text,
  PRIMARY KEY (`ApplicationID`),
  KEY `ApplicantID` (`ApplicantID`),
  KEY `VacancyID` (`VacancyID`),
  CONSTRAINT `applications_ibfk_1` FOREIGN KEY (`ApplicantID`) REFERENCES `Applicants` (`ApplicantID`) ON DELETE CASCADE,
  CONSTRAINT `applications_ibfk_2` FOREIGN KEY (`VacancyID`) REFERENCES `Job_Listings` (`VacancyID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Applications`
--

LOCK TABLES `Applications` WRITE;
/*!40000 ALTER TABLE `Applications` DISABLE KEYS */;
INSERT INTO `Applications` VALUES (1,2,2,'Accepted','2026-06-17',NULL),(2,3,3,'Rejected','2026-06-17',NULL),(3,7,2,'Rejected','2026-06-17',NULL),(4,7,4,'Accepted','2026-06-17',NULL),(5,8,1,'For Final Review','2026-06-17',NULL),(6,8,2,'Rejected','2026-06-17',NULL),(7,8,3,'Accepted','2026-06-17',NULL),(8,8,4,'Submitted','2026-06-17',NULL),(9,8,5,'Rejected','2026-06-17',NULL),(10,9,1,'Rejected','2026-06-17',NULL),(11,9,2,'For Final Review','2026-06-17',NULL),(12,9,3,'For Final Review','2026-06-17',NULL),(13,10,6,'Submitted','2026-06-17',NULL),(14,11,1,'For Final Review','2026-06-17',NULL),(15,8,6,'For Final Review','2026-06-17',NULL),(16,12,7,'For Final Review','2026-06-17',NULL),(17,13,6,'Submitted','2026-06-23',NULL);
/*!40000 ALTER TABLE `Applications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `applicationstatushistory`
--

DROP TABLE IF EXISTS `applicationstatushistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `applicationstatushistory` (
  `HistoryID` int NOT NULL AUTO_INCREMENT,
  `ApplicationID` int NOT NULL,
  `OldStatus` varchar(50) DEFAULT NULL,
  `NewStatus` varchar(50) NOT NULL,
  `Status` varchar(50) DEFAULT NULL,
  `Remarks` text,
  `ChangedAt` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`HistoryID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `applicationstatushistory`
--

LOCK TABLES `applicationstatushistory` WRITE;
/*!40000 ALTER TABLE `applicationstatushistory` DISABLE KEYS */;
INSERT INTO `applicationstatushistory` VALUES (1,7,'For Final Review','Accepted',NULL,'gud','2026-06-17 02:14:22'),(2,10,'For Final Review','Rejected',NULL,'bad','2026-06-17 02:14:33');
/*!40000 ALTER TABLE `applicationstatushistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Job_Listings`
--

DROP TABLE IF EXISTS `Job_Listings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Job_Listings` (
  `VacancyID` int NOT NULL AUTO_INCREMENT,
  `JobTitle` varchar(255) NOT NULL,
  `Department` varchar(100) NOT NULL,
  `JobDescription` text NOT NULL,
  `Qualifications` text NOT NULL,
  `Status` varchar(50) DEFAULT 'Active',
  PRIMARY KEY (`VacancyID`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Job_Listings`
--

LOCK TABLES `Job_Listings` WRITE;
/*!40000 ALTER TABLE `Job_Listings` DISABLE KEYS */;
INSERT INTO `Job_Listings` VALUES (1,'Junior C# Developer','IT Department','Looking for someone to build Avalonia apps.','Basic C# knowledge.','Closed'),(2,'Junior Python Developer','IT','Develop and maintain backend systems.','Proficiency in Python and relational databases.','Closed'),(3,'Database Administrator','IT','Optimize and manage local database instances.','Strong knowledge of MySQL optimization and schema management.','Closed'),(4,'QA Automation Engineer','IT','Build automated testing pipelines.','Experience with test scripts and QA frameworks.','Closed'),(5,'test','tester','test stuff','be able to test stuff','Active'),(6,'Electric Fan Manager','IT','Fix electric fans','PHD, TESDA, 25 Years Experience','Active'),(7,'Laptop Manager','Management','Manage laptops','PHD, 5 Years Experience, Laptop','Active');
/*!40000 ALTER TABLE `Job_Listings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Staff_Accounts`
--

DROP TABLE IF EXISTS `Staff_Accounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Staff_Accounts` (
  `StaffID` int NOT NULL AUTO_INCREMENT,
  `Email` varchar(255) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `FullName` varchar(255) DEFAULT NULL,
  `Birthdate` date DEFAULT NULL,
  `Bio` text,
  `Department` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`StaffID`),
  UNIQUE KEY `Email` (`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Staff_Accounts`
--

LOCK TABLES `Staff_Accounts` WRITE;
/*!40000 ALTER TABLE `Staff_Accounts` DISABLE KEYS */;
INSERT INTO `Staff_Accounts` VALUES (1,'hr@company.com','password123','John Recruiter',NULL,NULL,'HR'),(2,'admin@gmail.com','admin123','System Administrator',NULL,NULL,'Admin'),(3,'idkbro@gmail.com','911',NULL,NULL,NULL,NULL),(4,'please@gmail.com','1234',NULL,NULL,NULL,NULL),(5,'test5@gmail.com','00',NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `Staff_Accounts` ENABLE KEYS */;
UNLOCK TABLES;
SET @@SESSION.SQL_LOG_BIN = @MYSQLDUMP_TEMP_LOG_BIN;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-06-23  4:36:55
