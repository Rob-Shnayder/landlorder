-- ----------------------------------------------------------------------------
-- MySQL Workbench Migration
-- Migrated Schemata: landlorder
-- Source Schemata: landlorder
-- Created: Thu Aug 27 23:36:50 2015
-- ----------------------------------------------------------------------------

SET FOREIGN_KEY_CHECKS = 0;;

-- ----------------------------------------------------------------------------
-- Schema landlorder
-- ----------------------------------------------------------------------------
DROP SCHEMA IF EXISTS `landlorder` ;
CREATE SCHEMA IF NOT EXISTS `landlorder` ;

-- ----------------------------------------------------------------------------
-- Table landlorder.AspNetUsers
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `AspNetUsers` (
  `Id` VARCHAR(128) NOT NULL,
  `Email` VARCHAR(256) NULL,
  `EmailConfirmed` TINYINT(1) NOT NULL,
  `PasswordHash` LONGTEXT NULL,
  `SecurityStamp` LONGTEXT NULL,
  `PhoneNumber` LONGTEXT NULL,
  `PhoneNumberConfirmed` TINYINT(1) NOT NULL,
  `TwoFactorEnabled` TINYINT(1) NOT NULL,
  `LockoutEndDateUtc` DATETIME NULL,
  `LockoutEnabled` TINYINT(1) NOT NULL,
  `AccessFailedCount` INT NOT NULL,
  `UserName` VARCHAR(256) NULL,
  `FirstName` VARCHAR(50) NOT NULL DEFAULT '',
  `LastName` VARCHAR(100) NOT NULL DEFAULT '',
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `UserNameIndex` (`UserName`(255) ASC));

-- ----------------------------------------------------------------------------
-- Table landlorder.AspNetUserLogins
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `AspNetUserLogins` (
  `LoginProvider` VARCHAR(128) NOT NULL,
  `ProviderKey` VARCHAR(128) NOT NULL,
  `UserId` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`LoginProvider`, `ProviderKey`, `UserId`),
  INDEX `IX_UserId` (`UserId` ASC),
  CONSTRAINT `FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `landlorder`.`AspNetUsers` (`Id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION);

-- ----------------------------------------------------------------------------
-- Table landlorder.AspNetRoles
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `AspNetRoles` (
  `Id` VARCHAR(128) NOT NULL,
  `Name` VARCHAR(256) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `RoleNameIndex` (`Name`(255) ASC));

-- ----------------------------------------------------------------------------
-- Table landlorder.AspNetUserClaims
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `AspNetUserClaims` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `UserId` VARCHAR(128) NOT NULL,
  `ClaimType` LONGTEXT NULL,
  `ClaimValue` LONGTEXT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_UserId` (`UserId` ASC),
  CONSTRAINT `FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `landlorder`.`AspNetUsers` (`Id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION);

-- ----------------------------------------------------------------------------
-- Table landlorder.AspNetUserRoles
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `AspNetUserRoles` (
  `UserId` VARCHAR(128) NOT NULL,
  `RoleId` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`UserId`, `RoleId`),
  INDEX `IX_UserId` (`UserId` ASC),
  INDEX `IX_RoleId` (`RoleId` ASC),
  CONSTRAINT `FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId`
    FOREIGN KEY (`RoleId`)
    REFERENCES `landlorder`.`AspNetRoles` (`Id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `landlorder`.`AspNetUsers` (`Id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION);

-- ----------------------------------------------------------------------------
-- Table landlorder.__MigrationHistory
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `__MigrationHistory` (
  `MigrationId` VARCHAR(150) NOT NULL,
  `ContextKey` VARCHAR(300) NOT NULL,
  `Model` LONGBLOB NOT NULL,
  `ProductVersion` VARCHAR(32) NOT NULL,
  PRIMARY KEY (`MigrationId`, `ContextKey`(255)));

-- ----------------------------------------------------------------------------
-- Table landlorder.Review
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `Review` (
  `reviewID` INT NOT NULL AUTO_INCREMENT,
  `propertyID` INT NOT NULL,
  `userID` VARCHAR(128) NOT NULL,
  `rating` INT NOT NULL DEFAULT 0,
  `review` LONGTEXT NULL,
  `apartmentnum` VARCHAR(15) NULL,
  `landlordname` VARCHAR(100) NULL,
  `repairrating` INT NOT NULL,
  `communicationrating` INT NOT NULL,
  `date` DATE NOT NULL DEFAULT '2000-01-01',
  `anonymous` TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`reviewID`),
  CONSTRAINT `FK_Review_userID`
    FOREIGN KEY (`userID`)
    REFERENCES `landlorder`.`AspNetUsers` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

-- ----------------------------------------------------------------------------
-- Table landlorder.Property
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `landlorder`.`Property` (
  `propertyID` INT NOT NULL AUTO_INCREMENT,
  `streetaddress` VARCHAR(100) NOT NULL,
  `route` VARCHAR(250) NOT NULL DEFAULT 0,
  `city` VARCHAR(100) NOT NULL,
  `zip` VARCHAR(30) NOT NULL,
  `state` VARCHAR(30) NOT NULL,
  `country` VARCHAR(50) NOT NULL,
  `latitude` DECIMAL(9,6) NOT NULL DEFAULT 0.0,
  `longitude` DECIMAL(9,6) NOT NULL DEFAULT 0.0,
  `formatted_address` VARCHAR(500) NOT NULL DEFAULT '',
  PRIMARY KEY (`propertyID`));

-- ----------------------------------------------------------------------------
-- Table landlorder.zipinfo
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS`zipinfo` (
  `zip` CHAR(5) NOT NULL,
  `city` VARCHAR(64) NULL,
  `state` VARCHAR(10) NULL,
  `lat` DECIMAL(9,6) NOT NULL,
  `lng` DECIMAL(9,6) NOT NULL,
  PRIMARY KEY (`zip`));

-- ----------------------------------------------------------------------------
-- View landlorder.SearchReviews
-- ----------------------------------------------------------------------------
-- USE `landlorder`;
-- -- =============================================
-- -- Author:		Robert Shnayder
-- -- Create date: 7/13/2015
-- -- Description:	Takes user input and checks if a review exists based on the address. If no review exists, find surrounding locations reviews by zip or city.
-- -- =============================================
-- CREATE  OR REPLACE PROCEDURE [dbo].[SearchReviews] 
-- 	-- Add the parameters for the stored procedure here
-- 	@input varchar(MAX) = ''
-- 	
-- AS
-- BEGIN
-- 	-- SET NOCOUNT ON added to prevent extra result sets from
-- 	-- interfering with SELECT statements.
-- 	SET NOCOUNT ON;
-- 
--     -- Insert statements for procedure here
-- 	
-- 	DECLARE @comma VARCHAR(5)
-- 	SET @comma = ', '
-- 	DECLARE @street VARCHAR(50), @zip Varchar(5), @city varchar(50)
-- 	SET @street = '10326 sunstream lane'
-- 	SET @zip = '33428'
-- 	SET @city = 'boca raton'
-- 	
-- 	IF EXISTS(SELECT * FROM Property WHERE @street LIKE '%'+streetaddress+'%')
-- 	BEGIN
-- 	SELECT p.*, r.rating, r.review, r.landlordname, r.apartmentnum FROM Property p INNER JOIN Review r ON p.propertyID=r.propertyID WHERE @street LIKE '%'+streetaddress+'%'
-- 	PRINT N'Exact Match. ';
-- 	END
-- 	ELSE
-- 	BEGIN
-- 	SELECT p.*, r.rating, r.review, r.landlordname, r.apartmentnum FROM Property p INNER JOIN Review r ON p.propertyID=r.propertyID WHERE zip LIKE '%'+ @zip +'%' OR city LIKE '%'+ @city +'%'
-- 	PRINT N'Related Match.';
-- 	END	
-- 	
-- END
-- ;

-- ----------------------------------------------------------------------------
-- View landlorder.SearchReviews_StreetAddress
-- ----------------------------------------------------------------------------
-- USE `landlorder`;
-- -- =============================================
-- -- Author:		Robert Shnayder
-- -- Create date: 7/19/2015
-- -- Description:	Takes user input and checks if a review exists based on the street address. If no review exists, find surrounding locations reviews by zip or city.
-- -- =============================================
-- CREATE  OR REPLACE PROCEDURE [dbo].[SearchReviews_StreetAddress] 
-- 	-- Add the parameters for the stored procedure here
-- 	@streetaddress varchar(100),
-- 	@route varchar(250),
-- 	@city varchar(200),
-- 	@state varchar(10),
-- 	@postal_code varchar(100)	
-- 	
-- AS
-- BEGIN
-- 	-- SET NOCOUNT ON added to prevent extra result sets from
-- 	-- interfering with SELECT statements.
-- 	SET NOCOUNT ON;
-- 
--     -- Insert statements for procedure here
-- 		
-- 	SELECT Property.*, (SELECT count(*) FROM Review WHERE review.propertyID = property.propertyID) AS numofReviews
-- 	FROM Property 
-- 	WHERE @streetaddress LIKE '%'+streetaddress+'%' AND @route LIKE '%'+route+'%' AND @city LIKE '%'+city+'%'
-- 	
-- 	
-- 	--***Related Searches
-- 	--ELSE
-- 	--BEGIN
-- 	--	SELECT TOP 8 * FROM Property p  WHERE zip LIKE '%'+ @postal_code +'%' OR city LIKE '%'+ @city +'%'
-- 	--END
-- END
-- ;

-- ----------------------------------------------------------------------------
-- View landlorder.CalculateDistanceFromGeoCode
-- ----------------------------------------------------------------------------
-- USE `landlorder`;
-- --
-- -- This routine calculates the distance between two points (given the
-- -- latitude/longitude of those points). It is being used to calculate
-- -- the distance between two locations using GeoDataSource (TM) prodducts
-- --
-- -- Calculate distance between two points lat1, long1 and lat2, long2
-- -- Uses radius of earth in kilometers or miles as an argurments
-- --
-- -- Typical radius:  3963.0 (miles) (Default if no value specified)
-- --                  6387.7 (km)
-- --
-- --
-- -- For enquiries, please contact sales@geodatasource.com
-- -- Official Web site: http://www.geodatasource.com
-- --
-- -- Thanks to Janes Swarowski for contributing the source code.
-- --
-- -- GeoDataSource.com (C) All Rights Reserved 2015
-- --
-- 
-- 
-- CREATE  OR REPLACE function [dbo].[CalculateDistanceFromGeoCode]( @lat1 decimal(9,6) , @long1 decimal(9,6) , @lat2 decimal(9,6) , @long2 decimal(9,6))
-- returns float
-- 
-- as
-- 
-- begin
-- 
-- declare @DegToRad as float
-- declare @Ans as float
-- declare @Miles as float
-- 
-- set @DegToRad = 57.29577951
-- set @Ans = 0
-- set @Miles = 0
-- 
-- if @lat1 is null or @lat1 = 0 or @long1 is null or @long1 = 0 or @lat2 is
-- null or @lat2 = 0 or @long2 is null or @long2 = 0
-- 
-- begin
-- 
-- return ( @Miles )
-- 
-- end
-- 
-- set @Ans = SIN(@lat1 / @DegToRad) * SIN(@lat2 / @DegToRad) + COS(@lat1 / @DegToRad ) * COS( @lat2 / @DegToRad ) * COS(ABS(@long2 - @long1 )/@DegToRad)
-- 
-- set @Miles = 3959 * ATAN(SQRT(1 - SQUARE(@Ans)) / @Ans)
-- 
-- set @Miles = CEILING(@Miles)
-- 
-- return ( @Miles )
-- 
-- end
-- ;

-- ----------------------------------------------------------------------------
-- View landlorder.SearchReviews_StreetAddress_Related
-- ----------------------------------------------------------------------------
-- USE `landlorder`;
-- -- =============================================
-- -- Author:		Robert Shnayder
-- -- Create date: 7/19/2015
-- -- Description:	Takes user input and checks if a review exists based on the street address. If no review exists, find surrounding locations reviews by zip or city.
-- -- =============================================
-- CREATE  OR REPLACE PROCEDURE [dbo].[SearchReviews_StreetAddress_Related] 
-- 	-- Add the parameters for the stored procedure here
-- 	@lat decimal(9, 6),
-- 	@lon decimal(9, 6),
-- 	@ignorestreet varchar(50),
-- 	@ignoreroute varchar(50)	
-- 	
-- AS
-- BEGIN
-- 	-- SET NOCOUNT ON added to prevent extra result sets from
-- 	-- interfering with SELECT statements.
-- 	SET NOCOUNT ON;
-- 	
-- 	SELECT top 1000 Property.formatted_address, property.propertyID, 
-- 		(SELECT count(reviewID) FROM Review WHERE review.propertyID = property.propertyID) AS numofReviews,
-- 		(Select ROUND(AVG(CAST(rating AS FLOAT)), 2) FROM Review WHERE review.propertyID = property.propertyID) AS averageRating
-- 	FROM Property
-- 	INNER JOIN zipinfo z1 ON property.zip=z1.zip
-- 	WHERE Latitude BETWEEN (@lat - 0.3) AND (@lat + 0.3)
-- 		AND Longitude BETWEEN (@lon - 0.3) AND (@lon + 0.3)
-- 	ORDER BY dbo.CalculateDistanceFromGeocode(@lat, @lon, z1.lat, z1.lng)	
-- 
-- 	
-- END
-- ;

-- ----------------------------------------------------------------------------
-- View landlorder.InsertLatLng
-- ----------------------------------------------------------------------------
-- USE `landlorder`;
-- -- =============================================
-- -- Author:		Robert Shnayder
-- -- Create date: 7/13/2015
-- -- Description:	Takes user input and checks if a review exists based on the address. If no review exists, find surrounding locations reviews by zip or city.
-- -- =============================================
-- CREATE  OR REPLACE PROCEDURE [dbo].[InsertLatLng] 
-- 	-- Add the parameters for the stored procedure here
-- 	@lat decimal(9, 6),
-- 	@lng decimal(9, 6),
-- 	@address varchar(250)
-- AS
-- BEGIN
-- 	-- SET NOCOUNT ON added to prevent extra result sets from
-- 	-- interfering with SELECT statements.
-- 	IF EXISTS (SELECT * FROM Property WHERE formatted_address=@address)
--     UPDATE Property SET latitude=@lat, longitude=@lng WHERE formatted_address=@address
-- 	
-- END
-- ;

-- ----------------------------------------------------------------------------
-- Routine landlorder.SearchReviews
-- ----------------------------------------------------------------------------
-- DELIMITER $$
-- 
-- DELIMITER $$
-- USE `landlorder`$$
-- -- =============================================
-- -- Author:		Robert Shnayder
-- -- Create date: 7/13/2015
-- -- Description:	Takes user input and checks if a review exists based on the address. If no review exists, find surrounding locations reviews by zip or city.
-- -- =============================================
-- CREATE PROCEDURE [dbo].[SearchReviews] 
-- 	-- Add the parameters for the stored procedure here
-- 	@input varchar(MAX) = ''
-- 	
-- AS
-- BEGIN
-- 	-- SET NOCOUNT ON added to prevent extra result sets from
-- 	-- interfering with SELECT statements.
-- 	SET NOCOUNT ON;
-- 
--     -- Insert statements for procedure here
-- 	
-- 	DECLARE @comma VARCHAR(5)
-- 	SET @comma = ', '
-- 	DECLARE @street VARCHAR(50), @zip Varchar(5), @city varchar(50)
-- 	SET @street = '10326 sunstream lane'
-- 	SET @zip = '33428'
-- 	SET @city = 'boca raton'
-- 	
-- 	IF EXISTS(SELECT * FROM Property WHERE @street LIKE '%'+streetaddress+'%')
-- 	BEGIN
-- 	SELECT p.*, r.rating, r.review, r.landlordname, r.apartmentnum FROM Property p INNER JOIN Review r ON p.propertyID=r.propertyID WHERE @street LIKE '%'+streetaddress+'%'
-- 	PRINT N'Exact Match. ';
-- 	END
-- 	ELSE
-- 	BEGIN
-- 	SELECT p.*, r.rating, r.review, r.landlordname, r.apartmentnum FROM Property p INNER JOIN Review r ON p.propertyID=r.propertyID WHERE zip LIKE '%'+ @zip +'%' OR city LIKE '%'+ @city +'%'
-- 	PRINT N'Related Match.';
-- 	END	
-- 	
-- END
-- $$
-- 
-- DELIMITER ;
-- 
-- ----------------------------------------------------------------------------
-- Routine landlorder.SearchReviews_StreetAddress
-- ----------------------------------------------------------------------------
-- DELIMITER $$
-- 
-- DELIMITER $$
-- USE `landlorder`$$
-- -- =============================================
-- -- Author:		Robert Shnayder
-- -- Create date: 7/19/2015
-- -- Description:	Takes user input and checks if a review exists based on the street address. If no review exists, find surrounding locations reviews by zip or city.
-- -- =============================================
-- CREATE PROCEDURE [dbo].[SearchReviews_StreetAddress] 
-- 	-- Add the parameters for the stored procedure here
-- 	@streetaddress varchar(100),
-- 	@route varchar(250),
-- 	@city varchar(200),
-- 	@state varchar(10),
-- 	@postal_code varchar(100)	
-- 	
-- AS
-- BEGIN
-- 	-- SET NOCOUNT ON added to prevent extra result sets from
-- 	-- interfering with SELECT statements.
-- 	SET NOCOUNT ON;
-- 
--     -- Insert statements for procedure here
-- 		
-- 	SELECT Property.*, (SELECT count(*) FROM Review WHERE review.propertyID = property.propertyID) AS numofReviews
-- 	FROM Property 
-- 	WHERE @streetaddress LIKE '%'+streetaddress+'%' AND @route LIKE '%'+route+'%' AND @city LIKE '%'+city+'%'
-- 	
-- 	
-- 	--***Related Searches
-- 	--ELSE
-- 	--BEGIN
-- 	--	SELECT TOP 8 * FROM Property p  WHERE zip LIKE '%'+ @postal_code +'%' OR city LIKE '%'+ @city +'%'
-- 	--END
-- END
-- $$
-- 
-- DELIMITER ;
-- 
-- ----------------------------------------------------------------------------
-- Routine landlorder.SearchReviews_StreetAddress_Related
-- ----------------------------------------------------------------------------
-- DELIMITER $$
-- 
-- DELIMITER $$
-- USE `landlorder`$$
-- -- =============================================
-- -- Author:		Robert Shnayder
-- -- Create date: 7/19/2015
-- -- Description:	Takes user input and checks if a review exists based on the street address. If no review exists, find surrounding locations reviews by zip or city.
-- -- =============================================
-- CREATE PROCEDURE [dbo].[SearchReviews_StreetAddress_Related] 
-- 	-- Add the parameters for the stored procedure here
-- 	@lat decimal(9, 6),
-- 	@lon decimal(9, 6),
-- 	@ignorestreet varchar(50),
-- 	@ignoreroute varchar(50)	
-- 	
-- AS
-- BEGIN
-- 	-- SET NOCOUNT ON added to prevent extra result sets from
-- 	-- interfering with SELECT statements.
-- 	SET NOCOUNT ON;
-- 	
-- 	SELECT top 1000 Property.formatted_address, property.propertyID, 
-- 		(SELECT count(reviewID) FROM Review WHERE review.propertyID = property.propertyID) AS numofReviews,
-- 		(Select ROUND(AVG(CAST(rating AS FLOAT)), 2) FROM Review WHERE review.propertyID = property.propertyID) AS averageRating
-- 	FROM Property
-- 	INNER JOIN zipinfo z1 ON property.zip=z1.zip
-- 	WHERE Latitude BETWEEN (@lat - 0.3) AND (@lat + 0.3)
-- 		AND Longitude BETWEEN (@lon - 0.3) AND (@lon + 0.3)
-- 	ORDER BY dbo.CalculateDistanceFromGeocode(@lat, @lon, z1.lat, z1.lng)	
-- 
-- 	
-- END
-- $$
-- 
-- DELIMITER ;
-- 
-- ----------------------------------------------------------------------------
-- Routine landlorder.InsertLatLng
-- ----------------------------------------------------------------------------
-- DELIMITER $$
-- 
-- DELIMITER $$
-- USE `landlorder`$$
-- -- =============================================
-- -- Author:		Robert Shnayder
-- -- Create date: 7/13/2015
-- -- Description:	Takes user input and checks if a review exists based on the address. If no review exists, find surrounding locations reviews by zip or city.
-- -- =============================================
-- CREATE PROCEDURE [dbo].[InsertLatLng] 
-- 	-- Add the parameters for the stored procedure here
-- 	@lat decimal(9, 6),
-- 	@lng decimal(9, 6),
-- 	@address varchar(250)
-- AS
-- BEGIN
-- 	-- SET NOCOUNT ON added to prevent extra result sets from
-- 	-- interfering with SELECT statements.
-- 	IF EXISTS (SELECT * FROM Property WHERE formatted_address=@address)
--     UPDATE Property SET latitude=@lat, longitude=@lng WHERE formatted_address=@address
-- 	
-- END
-- $$
-- 
-- DELIMITER ;
-- 
-- ----------------------------------------------------------------------------
-- Routine landlorder.CalculateDistanceFromGeoCode
-- ----------------------------------------------------------------------------
-- DELIMITER $$
-- 
-- DELIMITER $$
-- USE `landlorder`$$
-- --
-- -- This routine calculates the distance between two points (given the
-- -- latitude/longitude of those points). It is being used to calculate
-- -- the distance between two locations using GeoDataSource (TM) prodducts
-- --
-- -- Calculate distance between two points lat1, long1 and lat2, long2
-- -- Uses radius of earth in kilometers or miles as an argurments
-- --
-- -- Typical radius:  3963.0 (miles) (Default if no value specified)
-- --                  6387.7 (km)
-- --
-- --
-- -- For enquiries, please contact sales@geodatasource.com
-- -- Official Web site: http://www.geodatasource.com
-- --
-- -- Thanks to Janes Swarowski for contributing the source code.
-- --
-- -- GeoDataSource.com (C) All Rights Reserved 2015
-- --
-- 
-- 
-- CREATE function [dbo].[CalculateDistanceFromGeoCode]( @lat1 decimal(9,6) , @long1 decimal(9,6) , @lat2 decimal(9,6) , @long2 decimal(9,6))
-- returns float
-- 
-- as
-- 
-- begin
-- 
-- declare @DegToRad as float
-- declare @Ans as float
-- declare @Miles as float
-- 
-- set @DegToRad = 57.29577951
-- set @Ans = 0
-- set @Miles = 0
-- 
-- if @lat1 is null or @lat1 = 0 or @long1 is null or @long1 = 0 or @lat2 is
-- null or @lat2 = 0 or @long2 is null or @long2 = 0
-- 
-- begin
-- 
-- return ( @Miles )
-- 
-- end
-- 
-- set @Ans = SIN(@lat1 / @DegToRad) * SIN(@lat2 / @DegToRad) + COS(@lat1 / @DegToRad ) * COS( @lat2 / @DegToRad ) * COS(ABS(@long2 - @long1 )/@DegToRad)
-- 
-- set @Miles = 3959 * ATAN(SQRT(1 - SQUARE(@Ans)) / @Ans)
-- 
-- set @Miles = CEILING(@Miles)
-- 
-- return ( @Miles )
-- 
-- end
-- $$
-- 
-- DELIMITER ;
-- SET FOREIGN_KEY_CHECKS = 1;;
