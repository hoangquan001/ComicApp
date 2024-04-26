--Tên: Hoàng Anh Quân 
--MSSV: 19120628
--DROP DATABASE ComicApp
CREATE DATABASE ComicApp
GO
USE ComicApp
GO

CREATE TABLE _USER (
    ID INT NOT NULL IDENTITY(1000,1),
    Username NVARCHAR(50) UNIQUE,
    Email NVARCHAR(100),
    HashPassword NVARCHAR(255),
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    DOB DATETIME,
    Avatar NVARCHAR(255),
    Gender int CHECK (Gender IN(0,1)) DEFAULT 0,
    Status int CHECK (Status IN(0,1)) DEFAULT 1,
    LastLogin DATETIME,
    LastLoginIp NVARCHAR(50),
    CreateAt DATETIME NOT NULL  DEFAULT CURRENT_TIMESTAMP,
    UpdateAt DATETIME NOT NULL  DEFAULT CURRENT_TIMESTAMP,
    Role int NOT NULL CHECK (Role IN(0,1,2)),
	CONSTRAINT PK_USER PRIMARY KEY (ID)
);

CREATE TABLE COMIC (
    ID INT NOT NULL IDENTITY(1000,1),
    Title NVARCHAR(255),
    URL NVARCHAR(255) NOT NULL UNIque,
    Author NVARCHAR(100),
    Description NVARCHAR(MAX),
    CoverImage NVARCHAR(255),
    Status int NOT NULL CHECK (status IN(0,1)),
    Rating int NOT NULL CHECK (Rating <=10),
    CreateAt DATETIME NOT NULL  DEFAULT CURRENT_TIMESTAMP,
    UpdateAt DATETIME NOT NULL  DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT PK_COMIC PRIMARY KEY (ID)
);

CREATE TABLE USER_FOLLOW_COMIC (
  UserID INT NOT NULL,
  ComicID INT NOT NULL,
  FollowDate DATETIME DEFAULT CURRENT_TIMESTAMP,  
  PRIMARY KEY (UserID, ComicID),
  FOREIGN KEY (UserID) REFERENCES _USER(ID),  
  FOREIGN KEY (ComicID) REFERENCES COMIC(ID) 
);

CREATE TABLE USER_VOTE_COMIC (
  UserID INT NOT NULL,
  ComicID INT NOT NULL,
  VotePoint INT NOT NULL CHECK(VotePoint in  (1,2,3,4,5,6,7,8,9,10)),
  PRIMARY KEY (UserID, ComicID),  
  FOREIGN KEY (UserID) REFERENCES _USER(ID),  
  FOREIGN KEY (ComicID) REFERENCES COMIC(ID)  
);


CREATE TABLE GENRE (
    ID INT NOT NULL,
    Title NVARCHAR(255) UNIQUE,
    Slug NVARCHAR(255) UNIQUE,
    Description NVARCHAR(MAX),
    CONSTRAINT PK_GENRE PRIMARY KEY (ID)
); 

CREATE TABLE COMIC_GENRE (
    ComicID INT,
    GenreID INT,
    CONSTRAINT PK_Comic_Genre PRIMARY KEY (ComicID, GenreID),
	CONSTRAINT FK_Comic_Genre_comic FOREIGN KEY (ComicID) REFERENCES COMIC(ID),
	CONSTRAINT FK_Comic_Genre_genre FOREIGN KEY (GenreID) REFERENCES GENRE(ID)
);



CREATE TABLE CHAPTER (
    ID INT NOT NULL UNIQUE IDENTITY(1,1) ,
    ComicID INT,
    Title NVARCHAR(255),
    ChapterNumber INT NOT NULL ,
    URL NVARCHAR(255),
    ViewCount INT NOT NULL DEFAULT 0,
    UpdateAt DATETIME NOT NULL  DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_CHAPTER PRIMARY KEY (ID),
    FOREIGN KEY (ComicID) REFERENCES COMIC(ID)
);

CREATE TABLE PAGE (
  ID INT NOT NULL IDENTITY(1,1),
  ChapterID INT NOT NULL,
  PageNumber INT NOT NULL,
  Content TEXT,  -- Description of the page content (optional)
  URL NVARCHAR(255),
  ImageURL VARCHAR(255),  -- URL for the page image
  CONSTRAINT FK_PAGE PRIMARY KEY (ID),
  FOREIGN KEY (ChapterID) REFERENCES CHAPTER(ID)
);


SELECT * 
FROM _USER
SELECT *
FROM COMIC
where id =29201

SELECT * 
FROM COMIC_GENRE
SELECT * 
FROM CHAPTER
SELECT * 
FROM GENRE
SELECT * 
FROM PAGE
DELETE FROM COMIC_GENRE WHERE COMICID>0

DELETE FROM COMIC WHERE id>=0;

DELETE FROM GENRE WHERE id >=1;

DELETE FROM CHAPTER WHERE id >=0;



SELECT c.*,t.*
FROM 
(Select comicId, Sum(ct.ViewCount) as 'view'
	from Chapter ct
	group by comicId
) as t, COMIC as c
where t.comicId = c.id
Order by 'view' desc


--UPDATE COMIC
--SET CreateAt = t.CreateAt
--FROM 
--(Select comicId, min(updateAt) as 'CreateAt'
--	from Chapter ct
--	group by comicId
--) as t, COMIC as c
--where t.comicId = c.id
CREATE FUNCTION GetComicView(@ComicId int)
RETURNS INT
AS
BEGIN
    declare @bav int
    select @bav = sum(viewCount) from Chapter where ComicId = @ComicId
    RETURN @bav
END
select cm.*, dbo.GetComicView(cm.id) as 'view'
from Comic cm
order by 'view' desc

Select *
from chapter
where updateat > '2020-08-04' and updateat > '2020-08-04'
