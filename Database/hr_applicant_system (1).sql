-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jun 07, 2026 at 12:35 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `hr_applicant_system`
--

-- --------------------------------------------------------

--
-- Table structure for table `applicants`
--

CREATE TABLE `applicants` (
  `ApplicantID` int(11) NOT NULL,
  `FullName` varchar(100) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `ContactNumber` varchar(30) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `applicants`
--

INSERT INTO `applicants` (`ApplicantID`, `FullName`, `Email`, `ContactNumber`, `CreatedAt`) VALUES
(1, 'Juan Dela Cruz', 'juan@example.com', NULL, '2026-06-05 16:22:58'),
(2, 'Maria Santos', 'maria@example.com', NULL, '2026-06-05 16:22:58'),
(3, 'Carlo Reyes', 'carlo@example.com', NULL, '2026-06-05 16:28:09'),
(4, 'Ana Cruz', 'ana@example.com', NULL, '2026-06-05 16:28:09'),
(5, 'Test Final Review Applicant', 'finalreview@example.com', '09123456789', '2026-06-05 17:45:23');

-- --------------------------------------------------------

--
-- Table structure for table `applications`
--

CREATE TABLE `applications` (
  `ApplicationID` int(11) NOT NULL,
  `ApplicantID` int(11) NOT NULL,
  `JobID` int(11) NOT NULL,
  `CurrentStatus` varchar(50) DEFAULT 'Submitted',
  `StaffReviewer` varchar(100) DEFAULT NULL,
  `ScreeningRemarks` text DEFAULT NULL,
  `AppliedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `applications`
--

INSERT INTO `applications` (`ApplicationID`, `ApplicantID`, `JobID`, `CurrentStatus`, `StaffReviewer`, `ScreeningRemarks`, `AppliedAt`) VALUES
(1, 1, 1, 'Accepted', 'HR Staff', 'Passed initial screening.', '2026-06-05 16:22:58'),
(2, 2, 1, 'Accepted', 'HR Staff', 'Qualified for final review.', '2026-06-05 16:22:58'),
(3, 3, 1, 'Rejected', 'HR Staff', 'Incomplete qualifications.', '2026-06-05 16:28:09'),
(4, 4, 1, 'Rejected', 'HR Staff', 'Missing required documents.', '2026-06-05 16:28:09'),
(5, 5, 2, 'Rejected', 'HR Staff', 'Passed initial screening and forwarded for final decision.', '2026-06-05 17:45:23');

-- --------------------------------------------------------

--
-- Table structure for table `applicationstatushistory`
--

CREATE TABLE `applicationstatushistory` (
  `HistoryID` int(11) NOT NULL,
  `ApplicationID` int(11) NOT NULL,
  `OldStatus` varchar(50) DEFAULT NULL,
  `NewStatus` varchar(50) DEFAULT NULL,
  `Remarks` text DEFAULT NULL,
  `ChangedBy` varchar(100) DEFAULT 'Admin/Manager',
  `ChangedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `applicationstatushistory`
--

INSERT INTO `applicationstatushistory` (`HistoryID`, `ApplicationID`, `OldStatus`, `NewStatus`, `Remarks`, `ChangedBy`, `ChangedAt`) VALUES
(1, 1, 'For Final Review', 'Accepted', '', 'Admin/Manager', '2026-06-05 16:26:20'),
(2, 2, 'For Final Review', 'Accepted', '', 'Admin/Manager', '2026-06-05 17:14:12'),
(3, 5, 'For Final Review', 'Rejected', 'missing documents', 'Admin/Manager', '2026-06-05 17:48:51');

-- --------------------------------------------------------

--
-- Table structure for table `hiringdecisions`
--

CREATE TABLE `hiringdecisions` (
  `DecisionID` int(11) NOT NULL,
  `ApplicationID` int(11) NOT NULL,
  `Decision` varchar(50) NOT NULL,
  `FinalRemarks` text DEFAULT NULL,
  `DecidedBy` varchar(100) DEFAULT 'Admin/Manager',
  `DecidedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `hiringdecisions`
--

INSERT INTO `hiringdecisions` (`DecisionID`, `ApplicationID`, `Decision`, `FinalRemarks`, `DecidedBy`, `DecidedAt`) VALUES
(1, 1, 'Accepted', '', 'Admin/Manager', '2026-06-05 16:26:20'),
(2, 2, 'Accepted', '', 'Admin/Manager', '2026-06-05 17:14:12'),
(3, 5, 'Rejected', 'missing documents', 'Admin/Manager', '2026-06-05 17:48:51');

-- --------------------------------------------------------

--
-- Table structure for table `jobvacancies`
--

CREATE TABLE `jobvacancies` (
  `JobID` int(11) NOT NULL,
  `JobTitle` varchar(100) NOT NULL,
  `Department` varchar(100) NOT NULL,
  `JobDescription` text NOT NULL,
  `MinimumQualifications` text NOT NULL,
  `VacancyStatus` varchar(20) DEFAULT 'Active',
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `jobvacancies`
--

INSERT INTO `jobvacancies` (`JobID`, `JobTitle`, `Department`, `JobDescription`, `MinimumQualifications`, `VacancyStatus`, `CreatedAt`) VALUES
(1, 'IT Support Staff', 'IT Department', 'Responsible for assisting employees with technical concerns.', 'Graduate of IT or related course.', 'Closed', '2026-06-05 16:21:06'),
(2, 'IT Staff', 'IT Department', 'Responsible for assisting employees with technical concerns.', 'IT Graduate', 'Active', '2026-06-05 17:13:39'),
(3, 'IT Manager', 'IT Department', 'handling staff and good personality', 'atleast 4 years of manager experience, IT college graduate', 'Active', '2026-06-06 22:35:43');

-- --------------------------------------------------------

--
-- Table structure for table `roles`
--

CREATE TABLE `roles` (
  `RoleID` int(11) NOT NULL,
  `RoleName` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `roles`
--

INSERT INTO `roles` (`RoleID`, `RoleName`) VALUES
(1, 'Admin'),
(2, 'HR Manager'),
(3, 'HR Staff');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `UserID` int(11) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Password` varchar(100) NOT NULL,
  `RoleID` int(11) NOT NULL,
  `FullName` varchar(100) DEFAULT NULL,
  `Birthdate` date DEFAULT NULL,
  `Bio` text DEFAULT NULL,
  `AccountStatus` varchar(20) DEFAULT 'Active',
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`UserID`, `Email`, `Password`, `RoleID`, `FullName`, `Birthdate`, `Bio`, `AccountStatus`, `CreatedAt`) VALUES
(1, 'azi1@gmil.com', '1234', 3, NULL, NULL, NULL, 'Active', '2026-06-05 16:13:55'),
(2, 'staff123@gmail.com', '1234', 3, NULL, NULL, NULL, 'Active', '2026-06-05 17:11:53'),
(3, 'admin@gmail.com', 'admin123', 1, 'System Admin', NULL, NULL, 'Active', '2026-06-05 17:45:23'),
(4, 'staff3@gmail.com', '654321', 3, NULL, NULL, NULL, 'Active', '2026-06-06 22:31:51');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `applicants`
--
ALTER TABLE `applicants`
  ADD PRIMARY KEY (`ApplicantID`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- Indexes for table `applications`
--
ALTER TABLE `applications`
  ADD PRIMARY KEY (`ApplicationID`),
  ADD KEY `ApplicantID` (`ApplicantID`),
  ADD KEY `JobID` (`JobID`);

--
-- Indexes for table `applicationstatushistory`
--
ALTER TABLE `applicationstatushistory`
  ADD PRIMARY KEY (`HistoryID`),
  ADD KEY `ApplicationID` (`ApplicationID`);

--
-- Indexes for table `hiringdecisions`
--
ALTER TABLE `hiringdecisions`
  ADD PRIMARY KEY (`DecisionID`),
  ADD KEY `ApplicationID` (`ApplicationID`);

--
-- Indexes for table `jobvacancies`
--
ALTER TABLE `jobvacancies`
  ADD PRIMARY KEY (`JobID`);

--
-- Indexes for table `roles`
--
ALTER TABLE `roles`
  ADD PRIMARY KEY (`RoleID`),
  ADD UNIQUE KEY `RoleName` (`RoleName`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`UserID`),
  ADD UNIQUE KEY `Email` (`Email`),
  ADD KEY `RoleID` (`RoleID`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `applicants`
--
ALTER TABLE `applicants`
  MODIFY `ApplicantID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `applications`
--
ALTER TABLE `applications`
  MODIFY `ApplicationID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `applicationstatushistory`
--
ALTER TABLE `applicationstatushistory`
  MODIFY `HistoryID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `hiringdecisions`
--
ALTER TABLE `hiringdecisions`
  MODIFY `DecisionID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `jobvacancies`
--
ALTER TABLE `jobvacancies`
  MODIFY `JobID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `roles`
--
ALTER TABLE `roles`
  MODIFY `RoleID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `UserID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `applications`
--
ALTER TABLE `applications`
  ADD CONSTRAINT `applications_ibfk_1` FOREIGN KEY (`ApplicantID`) REFERENCES `applicants` (`ApplicantID`),
  ADD CONSTRAINT `applications_ibfk_2` FOREIGN KEY (`JobID`) REFERENCES `jobvacancies` (`JobID`);

--
-- Constraints for table `applicationstatushistory`
--
ALTER TABLE `applicationstatushistory`
  ADD CONSTRAINT `applicationstatushistory_ibfk_1` FOREIGN KEY (`ApplicationID`) REFERENCES `applications` (`ApplicationID`);

--
-- Constraints for table `hiringdecisions`
--
ALTER TABLE `hiringdecisions`
  ADD CONSTRAINT `hiringdecisions_ibfk_1` FOREIGN KEY (`ApplicationID`) REFERENCES `applications` (`ApplicationID`);

--
-- Constraints for table `users`
--
ALTER TABLE `users`
  ADD CONSTRAINT `users_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `roles` (`RoleID`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
