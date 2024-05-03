CREATE DATABASE  IF NOT EXISTS `planit_db` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `planit_db`;
-- MySQL dump 10.13  Distrib 8.0.34, for Win64 (x86_64)
--
-- Host: localhost    Database: planit_db
-- ------------------------------------------------------
-- Server version	8.0.35

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

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20240429103859_PlanITMigrations','8.0.3');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Contacts`
--

DROP TABLE IF EXISTS `Contacts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Contacts` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Contacts_UserId_Email` (`UserId`,`Email`),
  CONSTRAINT `FK_Contacts_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Contacts`
--

LOCK TABLES `Contacts` WRITE;
/*!40000 ALTER TABLE `Contacts` DISABLE KEYS */;
INSERT INTO `Contacts` VALUES (1,1,'Kari Nordmann','karinordmann@mail.com'),(2,1,'Emma Larsen','emmalarsen@mail.com'),(3,1,'Siri Nilsen','sirinilsen@mail.com'),(4,1,'Erik Andersen','erikandersen@mail.com'),(5,1,'Jonas Pedersen','jonaspedersen@mail.com'),(6,1,'Sara Hansen','sarahansen@mail.com'),(7,1,'Lars Lie','larslie@mail.com'),(8,2,'Per Hansen','perhansen@mail.com'),(9,2,'Lars Lie','larslie@mail.com'),(10,2,'Ingrid Dahl','ingriddahl@mail.com'),(11,3,'Per Hansen','perhansen@mail.com'),(12,3,'Emma Larsen','emmalarsen@mail.com'),(13,3,'Magnus Dahl','magnusdahl@mail.com');
/*!40000 ALTER TABLE `Contacts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Dinners`
--

DROP TABLE IF EXISTS `Dinners`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Dinners` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `Date` date NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Dinners_UserId` (`UserId`),
  CONSTRAINT `FK_Dinners_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=50 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Dinners`
--

LOCK TABLES `Dinners` WRITE;
/*!40000 ALTER TABLE `Dinners` DISABLE KEYS */;
INSERT INTO `Dinners` VALUES (1,1,'2024-05-02','Pizza'),(2,1,'2024-05-06','Pasta carbonara'),(3,1,'2024-05-07','Chicken fajitas'),(4,1,'2024-05-08','Paella'),(5,1,'2024-05-09','Caesar salad'),(6,1,'2024-05-10','Swedish meatballs'),(7,1,'2024-05-11','Chicken fried rice'),(8,1,'2024-05-12','Tomato soup'),(9,1,'2024-05-13','Lasagna'),(10,1,'2024-05-14','Chicken & noodle stir-fry'),(11,1,'2024-05-15','Chili con carne'),(12,1,'2024-05-16','Fish & chips'),(13,1,'2024-05-17','Takeaway'),(14,1,'2024-05-18','Hamburgers'),(15,1,'2024-05-19','Sunday roast'),(16,1,'2024-05-20','Leftovers'),(17,1,'2024-05-21','Spagetti bolognese'),(18,1,'2024-05-22','Pancakes'),(19,1,'2024-05-23','Tacos'),(20,1,'2024-05-24','Meatloaf'),(21,1,'2024-05-25','Chicken nuggets & rice'),(22,1,'2024-05-26','Beef stroganoff '),(23,1,'2024-05-27','BLT sandwiches'),(24,1,'2024-05-28','Meatball subs'),(25,1,'2024-05-29','Takeaway'),(26,1,'2024-05-30','Shrimp alfredo'),(27,1,'2024-05-31','Pizza'),(28,2,'2024-05-06','Hamburgers'),(29,2,'2024-05-07','Pancakes'),(30,2,'2024-05-08','Turkey roast'),(31,2,'2024-05-09','Pasta pesto'),(32,2,'2024-05-10','Tacos'),(33,2,'2024-05-11','Lobster'),(34,2,'2024-05-12','Chicken teriyaki'),(35,2,'2024-05-13','Leftovers'),(36,2,'2024-05-14','KFC'),(37,2,'2024-05-15','BBQ ribs'),(38,2,'2024-05-16','Pasta alfredo'),(39,3,'2024-05-06','Pizza'),(40,3,'2024-05-07','Meat pie'),(41,3,'2024-05-08','Chicken korma'),(42,3,'2024-05-09','Shrimp tacos'),(43,3,'2024-05-10','Takeaway'),(44,3,'2024-05-11','Chili sin carne'),(45,3,'2024-05-12','Salmon stir-fry'),(46,3,'2024-05-13','Pancakes'),(47,3,'2024-05-17','Pizza'),(48,3,'2024-05-16','Chicken tandoori'),(49,3,'2024-05-18','Tomato soup');
/*!40000 ALTER TABLE `Dinners` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Events`
--

DROP TABLE IF EXISTS `Events`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Events` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Date` date NOT NULL,
  `Time` time(6) NOT NULL,
  `Location` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Events_UserId` (`UserId`),
  CONSTRAINT `FK_Events_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Events`
--

LOCK TABLES `Events` WRITE;
/*!40000 ALTER TABLE `Events` DISABLE KEYS */;
INSERT INTO `Events` VALUES (1,1,'Birthday Party','2024-06-06','18:30:00.000000','at home'),(2,1,'Dinner Date','2024-07-07','19:00:00.000000','Peppes Pizza'),(3,1,'Flea Market','2024-06-11','10:00:00.000000','Town square'),(4,1,'Graduation party','2024-06-19','11:30:00.000000','Scandic Park Hotell'),(5,1,'Wedding','2024-08-17','15:00:00.000000','Sandefjord church'),(6,2,'Garden Party','2024-06-08','15:00:00.000000','At home'),(7,2,'Dinner date','2024-08-24','18:45:00.000000','Maaemo'),(8,3,'Graduation Party','2024-06-19','11:30:00.000000','Scandic Park Hotel');
/*!40000 ALTER TABLE `Events` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ImportantDates`
--

DROP TABLE IF EXISTS `ImportantDates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ImportantDates` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Date` date NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ImportantDates_UserId` (`UserId`),
  CONSTRAINT `FK_ImportantDates_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ImportantDates`
--

LOCK TABLES `ImportantDates` WRITE;
/*!40000 ALTER TABLE `ImportantDates` DISABLE KEYS */;
INSERT INTO `ImportantDates` VALUES (1,1,'National Day','2024-05-17'),(2,1,'Graduation','2024-06-19'),(3,1,'Emma\'s birthday','2024-11-16'),(4,1,'Father\'s day','2024-11-10'),(5,1,'Wedding anniversary','2024-05-29'),(6,2,'Vacation start','2024-06-03'),(7,2,'Denmark trip','2024-06-12'),(8,2,'Per\'s birthday','2024-10-23'),(9,3,'National Day','2024-05-17'),(10,3,'Simon\'s Wedding','2024-09-28');
/*!40000 ALTER TABLE `ImportantDates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Invites`
--

DROP TABLE IF EXISTS `Invites`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Invites` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `EventId` int NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsReminderSent` tinyint(1) NOT NULL,
  `Coming` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Invites_Email_EventId` (`Email`,`EventId`),
  KEY `IX_Invites_EventId` (`EventId`),
  CONSTRAINT `FK_Invites_Events_EventId` FOREIGN KEY (`EventId`) REFERENCES `Events` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Invites`
--

LOCK TABLES `Invites` WRITE;
/*!40000 ALTER TABLE `Invites` DISABLE KEYS */;
INSERT INTO `Invites` VALUES (1,1,'Kari Nordmann','karinordmann@mail.com',1,1),(2,1,'Emma Larsen','emmalarsen@mail.com',1,1),(3,1,'Siri Nilsen','sirinilsen@mail.com',1,1),(4,1,'Lars Lie','larslie@mail.com',1,0),(5,2,'Sara Hansen','sarahansen@mail.com',1,0),(6,2,'Erik Andersen','erikandersen@mail.com',1,1),(7,2,'Siri Nilsen','sirinilsen@mail.com',1,1),(8,3,'Lars Lie','larslie@mail.com',1,0),(9,3,'Sara Hansen','sarahansen@mail.com',1,1),(10,4,'Magnus Dahl','magnusdahl@mail.com',1,1),(11,4,'Ingrid Dahl','ingriddahl@mail.com',1,0),(12,4,'Marius Solberg','mariussolberg@mail.com',1,1),(13,6,'Per Hansen','perhansen@mail.com',1,1),(14,6,'Siri Nilsen','sirinilsen@mail.com',1,1),(15,6,'Lars Lie','larslie@mail.com',1,0),(16,7,'Sara Hansen','sarahansen@mail.com',1,0),(17,7,'Erik Andersen','erikandersen@mail.com',1,1),(18,8,'Per Hansen','perhansen@mail.com',1,1),(19,8,'Lars Lie','larslie@mail.com',1,0),(20,8,'Sara Hansen','sarahansen@mail.com',1,1),(21,8,'Erik Andersen','erikandersen@mail.com',1,1);
/*!40000 ALTER TABLE `Invites` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Shoppinglists`
--

DROP TABLE IF EXISTS `Shoppinglists`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Shoppinglists` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ShoppingLists_UserId` (`UserId`),
  CONSTRAINT `FK_ShoppingLists_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Shoppinglists`
--

LOCK TABLES `Shoppinglists` WRITE;
/*!40000 ALTER TABLE `Shoppinglists` DISABLE KEYS */;
INSERT INTO `Shoppinglists` VALUES (1,1,'Milk'),(3,1,'Eggs'),(4,1,'Bread'),(5,1,'Butter'),(6,1,'Cheese'),(7,1,'Coffee'),(8,1,'Onions'),(9,1,'Tomatoes'),(10,1,'Bananas'),(11,2,'Bacon'),(12,2,'Rice'),(13,2,'Apples'),(14,2,'Soap'),(15,2,'Bananas'),(16,2,'Salt'),(17,2,'Cooking oil'),(18,3,'Milk'),(19,3,'Cookies'),(20,3,'Oranges'),(21,3,'Pepper');
/*!40000 ALTER TABLE `Shoppinglists` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Todos`
--

DROP TABLE IF EXISTS `Todos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Todos` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Todos_UserId` (`UserId`),
  CONSTRAINT `FK_Todos_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Todos`
--

LOCK TABLES `Todos` WRITE;
/*!40000 ALTER TABLE `Todos` DISABLE KEYS */;
INSERT INTO `Todos` VALUES (1,1,'Clean the car'),(2,1,'Laundry'),(3,1,'Pay bills'),(4,1,'Grocery shopping'),(5,1,'Water the plants'),(6,1,'Organize the garage'),(7,1,'Mow the lawn'),(8,1,'Wash the dishes'),(9,1,'Take out the trash'),(10,1,'Walk the dog'),(11,2,'Water plants'),(12,2,'Clean the kitchen'),(13,2,'Exercise'),(14,2,'Bake cookies'),(15,3,'Laundry'),(16,3,'Pay bills'),(17,3,'Clean Bathroom');
/*!40000 ALTER TABLE `Todos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Users`
--

DROP TABLE IF EXISTS `Users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `HashedPassword` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Salt` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Users_Email` (`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Users`
--

LOCK TABLES `Users` WRITE;
/*!40000 ALTER TABLE `Users` DISABLE KEYS */;
INSERT INTO `Users` VALUES (1,'perhansen@mail.com','Per Hansen','$2a$11$78UvIkzRPoPjJRzRlchEK.me1Hs8Ya5Wic3oQhl.H42yL6sGy6fxm','$2a$11$78UvIkzRPoPjJRzRlchEK.'),(2,'emmalarsen@mail.com','Emma Larsen','$2a$11$7J7bKFueAMOZSkhCwFhWLes0EuOwmtUslxtFEy3k6L5Fj5oE6eFe.','$2a$11$7J7bKFueAMOZSkhCwFhWLe'),(3,'sirinilsen@mail.com','Siri Nilsen','$2a$11$Hilc3ZvrfpLF7Rst/hFWY.UHk2e/6qtpjDEdPHiOQqn6fct0.9/G.','$2a$11$Hilc3ZvrfpLF7Rst/hFWY.');
/*!40000 ALTER TABLE `Users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-05-02 16:48:49
