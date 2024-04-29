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
    OtherName VARCHAR(255),
    URL VARCHAR(255) NOT NULL UNIQUE,
    Author VARCHAR(100),
    Description TEXT,
    CoverImage VARCHAR(255),
    Status INT NOT NULL CHECK (status IN (0, 1)),
    Rating NUMERIC NOT NULL CHECK (Rating <= 10),
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

CREATE TABLE IF NOT EXISTS hostcollector
(
    id int NOT NULL,
    host VARCHAR(255),
    comic_format VARCHAR(255),
    chapter_format VARCHAR(255),
    CONSTRAINT hostcollector_pkey PRIMARY KEY (id)
)

CREATE TABLE IF NOT EXISTS chaptermapping
(
    comic_slug VARCHAR(255)  NOT NULL,
    chapter_slug VARCHAR(255)  NOT NULL,
    chapterid VARCHAR(255) ,
    comicid VARCHAR(255) ,
    hostid integer,
    CONSTRAINT chaptermapping_pkey PRIMARY KEY (comic_slug, chapter_slug),
    CONSTRAINT fk_hostid FOREIGN KEY (hostid) REFERENCES hostcollector (id)
)
CREATE INDEX idx_user_username ON _USER (Username);
CREATE INDEX idx_user_email ON _USER (Email);
CREATE INDEX idx_user_role ON _USER (Role);

CREATE INDEX idx_COMIC_title ON COMIC (Title);
CREATE INDEX idx_COMIC_status ON COMIC (Status);
CREATE INDEX idx_COMIC_rating ON COMIC (Rating);

CREATE INDEX idx_genre_title ON GENRE (Title);
CREATE INDEX idx_genre_slug ON GENRE (Slug);

CREATE INDEX idx_comic_id ON CHAPTER (ComicID);
CREATE INDEX idx_chapter_number ON CHAPTER (ChapterNumber);

CREATE INDEX idx_comic_genre_comic_id ON COMIC_GENRE (ComicID);
CREATE INDEX idx_comic_genre_genre_id ON COMIC_GENRE (GenreID);

CREATE INDEX idx_chapter_id ON PAGE (ChapterID);




INSERT INTO hostcollector
(id,host,comic_format,chapter_format)
VALUES
(1,'nhattruyenss.com','https://{host}/truyen-tranh/{comic-slug}-{comicid}','https://{host}/truyen-tranh/{comic-slug}/{chapter-slug}/{chapterid}');