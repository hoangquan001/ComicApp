--Tên: Hoàng Anh Quân 
--MSSV: 19120628
--DROP DATABASE ComicApp
CREATE DATABASE ComicApp
GO
USE ComicApp
GO

CREATE TABLE _USER (
    ID SERIAL PRIMARY KEY,
    Username VARCHAR(50) UNIQUE,
    Email VARCHAR(100),
    HashPassword VARCHAR(255),
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    DOB TIMESTAMP,
    Avatar VARCHAR(255),
    Gender INT CHECK (Gender IN (0,1)) DEFAULT 0,
    Status INT CHECK (Status IN (0,1)) DEFAULT 1,
    LastLogin TIMESTAMP,
    LastLoginIp VARCHAR(50),
    CreateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Role INT NOT NULL CHECK (Role IN (0,1,2))
);

CREATE TABLE COMIC (
    ID SERIAL PRIMARY KEY,
    Title VARCHAR(255),
    URL VARCHAR(255) UNIQUE NOT NULL,
    Author VARCHAR(100),
    Description TEXT,
    CoverImage VARCHAR(255),
    Status INT NOT NULL CHECK (Status IN (0,1)),
    Rating INT NOT NULL CHECK (Rating <= 10),
    CreateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE GENRE (
    ID SERIAL PRIMARY KEY,
    Title VARCHAR(255) UNIQUE,
    Slug VARCHAR(255) UNIQUE,
    Description TEXT
);

CREATE TABLE COMIC_GENRE (
    ComicID INT REFERENCES COMIC(ID),
    GenreID INT REFERENCES GENRE(ID),
    PRIMARY KEY (ComicID, GenreID)
);

CREATE TABLE CHAPTER (
    ID SERIAL PRIMARY KEY,
    ComicID INT REFERENCES COMIC(ID),
    Title VARCHAR(255),
    ChapterNumber INT NOT NULL,
    URL VARCHAR(255),
    ViewCount INT NOT NULL DEFAULT 0,
    UpdateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE COMIC_GENRE (
    ComicID INT,
    GenreID INT,
    CONSTRAINT PK_Comic_Genre PRIMARY KEY (ComicID, GenreID),
	CONSTRAINT FK_Comic_Genre_comic FOREIGN KEY (ComicID) REFERENCES COMIC(ID),
	CONSTRAINT FK_Comic_Genre_genre FOREIGN KEY (GenreID) REFERENCES GENRE(ID)
);





-- CREATE TABLE PAGE (
--   ID INT NOT NULL IDENTITY(1,1),
--   ChapterID INT NOT NULL,
--   PageNumber INT NOT NULL,
--   Content TEXT,  -- Description of the page content (optional)
--   URL NVARCHAR(255),
--   ImageURL VARCHAR(255),  -- URL for the page image
--   CONSTRAINT FK_PAGE PRIMARY KEY (ID),
--   FOREIGN KEY (ChapterID) REFERENCES CHAPTER(ID)
-- );


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
