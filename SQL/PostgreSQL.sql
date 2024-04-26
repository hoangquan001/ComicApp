--DROP DATABASE IF EXISTS ComicApp;
CREATE DATABASE ComicApp;

\c ComicApp;

CREATE TABLE _USER (
    ID SERIAL PRIMARY KEY,
    Username VARCHAR(50) UNIQUE,
    Email VARCHAR(100),
    HashPassword VARCHAR(255),
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    DOB TIMESTAMP,
    Avatar VARCHAR(255),
    Gender INT CHECK (Gender IN (0, 1)) DEFAULT 0,
    Status INT CHECK (Status IN (0, 1)) DEFAULT 1,
    LastLogin TIMESTAMP,
    LastLoginIp VARCHAR(50),
    CreateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Role INT NOT NULL CHECK (Role IN (0, 1, 2))
);

CREATE TABLE COMIC (
    ID SERIAL PRIMARY KEY,
    Title VARCHAR(255),
    URL VARCHAR(255) NOT NULL UNIQUE,
    Author VARCHAR(100),
    Description TEXT,
    CoverImage VARCHAR(255),
    Status INT NOT NULL CHECK (status IN (0, 1)),
    Rating INT NOT NULL CHECK (Rating <= 10),
    CreateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE USER_FOLLOW_COMIC (
  UserID INT NOT NULL,
  ComicID INT NOT NULL,
  FollowDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,  
  PRIMARY KEY (UserID, ComicID),
  FOREIGN KEY (UserID) REFERENCES _USER(ID),  
  FOREIGN KEY (ComicID) REFERENCES COMIC(ID) 
);

CREATE TABLE USER_VOTE_COMIC (
  UserID INT NOT NULL,
  ComicID INT NOT NULL,
  VotePoint INT NOT NULL CHECK (VotePoint IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10)),
  PRIMARY KEY (UserID, ComicID),  
  FOREIGN KEY (UserID) REFERENCES _USER(ID),  
  FOREIGN KEY (ComicID) REFERENCES COMIC(ID)  
);

CREATE TABLE GENRE (
    ID SERIAL PRIMARY KEY,
    Title VARCHAR(255) UNIQUE,
    Slug VARCHAR(255) UNIQUE,
    Description TEXT
);

CREATE TABLE COMIC_GENRE (
    ComicID INT,
    GenreID INT,
    PRIMARY KEY (ComicID, GenreID),
    FOREIGN KEY (ComicID) REFERENCES COMIC(ID),
    FOREIGN KEY (GenreID) REFERENCES GENRE(ID)
);

CREATE TABLE CHAPTER (
    ID SERIAL PRIMARY KEY,
    ComicID INT,
    Title VARCHAR(255),
    ChapterNumber INT NOT NULL,
    URL VARCHAR(255),
    ViewCount INT NOT NULL DEFAULT 0,
    UpdateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ComicID) REFERENCES COMIC(ID)
);

CREATE TABLE PAGE (
  ID SERIAL PRIMARY KEY,
  ChapterID INT NOT NULL,
  PageNumber INT NOT NULL,
  Content TEXT,  -- Description of the page content (optional)
  URL VARCHAR(255),
  ImageURL VARCHAR(255),  -- URL for the page image
  FOREIGN KEY (ChapterID) REFERENCES CHAPTER(ID)
);

-- Select Queries
SELECT * FROM _USER;
SELECT * FROM COMIC WHERE id = 29201;
SELECT * FROM COMIC_GENRE;
SELECT * FROM CHAPTER;
SELECT * FROM GENRE;
SELECT * FROM PAGE;

-- Delete Queries
DELETE FROM COMIC_GENRE WHERE COMICID > 0;
DELETE FROM COMIC WHERE id >= 0;
DELETE FROM GENRE WHERE id >= 1;
DELETE FROM CHAPTER WHERE id >= 0;

-- View Count Query
SELECT c.*, t.view
FROM (
    SELECT comicId, SUM(ct.ViewCount) AS view
    FROM Chapter ct
    GROUP BY comicId
) AS t
JOIN COMIC AS c ON t.comicId = c.id
ORDER BY t.view DESC;

-- Function Definition
CREATE OR REPLACE FUNCTION GetComicView(ComicId INT) RETURNS INT AS $$
BEGIN
    RETURN (SELECT SUM(viewCount) FROM Chapter WHERE ComicId = GetComicView.ComicId);
END;
$$ LANGUAGE plpgsql;

-- Function Usage
SELECT cm.*, GetComicView(cm.id) AS view
FROM Comic cm
ORDER BY view DESC;

-- Date Comparison Query
SELECT *
FROM chapter
WHERE updateat > '2020-08-04';



INSERT INTO hostcollector
(id,host,comic_format,chapter_format)
VALUES
(1,'nhattruyenss.com','https://{host}/truyen-tranh/{comic-slug}-{comicid}','https://{host}/truyen-tranh/{comic-slug}/{chapter-slug}/{chapterid}')