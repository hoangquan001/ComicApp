--DROP DATABASE IF EXISTS ComicApp;
CREATE DATABASE ComicApp;

\c ComicApp;

CREATE TABLE _USER (
    ID SERIAL PRIMARY KEY,
    -- Username VARCHAR(50) UNIQUE,
    Email VARCHAR(100) UNIQUE,
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
    Role INT NOT NULL CHECK (Role IN (0, 1, 2)),
    Experience INT DEFAULT 0,
    Maxim TEXT,
    TypeLevel INT DEFAULT 0
    --MOCK DATA
    -- INSERT INTO _USER (Username, Email, FirstName, LastName, DOB, Avatar, Gender, Status, Role)
    -- VALUES
    -- ('johndoe', 'johndoe@example.com', 'John', 'Doe', '1990-01-01 00:00:00', 'avatar.jpg', 0, 1, 0),
    -- ('janedoe', 'janedoe@example.com', 'Jane', 'Doe', '1995-02-15 00:00:00', 'avatar2.png', 1, 1, 1),
    -- ('coderDude', 'coderdude@gmail.com', 'Alex', 'Wang', '1985-12-24 00:00:00', 'avatar3.gif', 0, 1, 2),
    -- ('bookworm', 'bookworm@hotmail.com', 'Alice', 'Green', '2000-06-09 00:00:00', NULL, 1, 0, 0),
    -- ('musicLover', 'musiclover@yahoo.com', 'Ben', 'Garcia', '1978-03-18 00:00:00', 'avatar5.jpg', 0, 1, 1);
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
    UpdateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    viewcount integer DEFAULT 0,
    numchapter integer NOT NULL DEFAULT 0
    lastchapter INT
    FOREIGN KEY (lastchapter) REFERENCES CHAPTER(ID) ON  DELETE SET NULL
);
-- ALTER TABLE comic ADD  FOREIGN KEY (lastchapter) REFERENCES CHAPTER(id) ON  DELETE SET NULL

CREATE TABLE USER_FOLLOW_COMIC (
  UserID INT NOT NULL,
  ComicID INT NOT NULL,  
  CreateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

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
    FOREIGN KEY (ComicID) REFERENCES COMIC(ID) ON DELETE CASCADE;,
    FOREIGN KEY (GenreID) REFERENCES GENRE(ID)
);

CREATE TABLE CHAPTER (
    ID SERIAL PRIMARY KEY,
    ComicID INT,
    Title VARCHAR(255),
    URL REAL,
    ViewCount INT NOT NULL DEFAULT 0,
    UpdateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Pages TEXT, 
    FOREIGN KEY (ComicID) REFERENCES COMIC(ID) ON DELETE CASCADE;
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
CREATE TABLE DAILY_COMIC_VIEWS (
    ComicID INT NOT NULL,
    ViewDate DATE NOT NULL,
    ViewCount INT NOT NULL DEFAULT 0,
    PRIMARY KEY (ComicID, ViewDate),
    FOREIGN KEY (ComicID) REFERENCES COMIC(ID)
);
-- SELECT ComicID, ViewDate, ViewCount
-- FROM DAILY_COMIC_VIEWS
-- WHERE ViewDate = CURRENT_DATE;
-- SELECT ComicID, DATE_TRUNC('week', ViewDate) as ViewWeek, SUM(ViewCount) as ViewCount
-- FROM DAILY_COMIC_VIEWS
-- WHERE ViewDate >= CURRENT_DATE - INTERVAL '1 week'
-- GROUP BY ComicID, DATE_TRUNC('week', ViewDate);
-- SELECT ComicID, DATE_TRUNC('month', ViewDate) as ViewMonth, SUM(ViewCount) as ViewCount
-- FROM DAILY_COMIC_VIEWS
-- WHERE ViewDate >= CURRENT_DATE - INTERVAL '1 month'
-- GROUP BY ComicID, DATE_TRUNC('month', ViewDate);

CREATE TABLE COMMENT (
  ID SERIAL PRIMARY KEY,
  ChapterID INT NOT NULL,
  ComicID INT NOT NULL,
  UserID INT NOT NULL,
  Content TEXT,
  CommentedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  ParentCommentID INT DEFAULT NULL,
  ReplyUserID INT DEFAULT NULL,
  FOREIGN KEY (ChapterID) REFERENCES CHAPTER(ID),
  FOREIGN KEY (UserID) REFERENCES _USER(ID),
  FOREIGN KEY (ParentCommentID) REFERENCES COMMENT(ID),
  FOREIGN KEY (ComicID) REFERENCES COMIC(ID)
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
-- Create the USER NOTIFICATION table
CREATE TABLE IF NOT EXISTS USER_NOTIFICATION (
    UserID INT NOT NULL,
    NtfID INT NOT NULL,
    IsRead BOOLEAN NOT NULL DEFAULT FALSE,
    PRIMARY KEY (UserID, NtfID),
    FOREIGN KEY (UserID) REFERENCES _USER(ID),
    FOREIGN KEY (NtfID) REFERENCES NOTIFICATION(ID)
);

CREATE TABLE IF NOT EXISTS NOTIFICATION (
    ID SERIAL PRIMARY KEY,
    Message TEXT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Type INT default 0,
    Params TEXT
);

CREATE TABLE IF NOT EXISTS NEW_COMIC_RAW (
    URL VARCHAR(255) PRIMARY KEY,
    DATA TEXT,
    STATUS INT,
    CREATEDAT TIMESTAMP
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
CREATE INDEX idx_chapter_number ON CHAPTER (url);

CREATE INDEX idx_comic_genre_comic_id ON COMIC_GENRE (ComicID);
CREATE INDEX idx_comic_genre_genre_id ON COMIC_GENRE (GenreID);

CREATE INDEX idx_chapter_id ON PAGE (ChapterID);

CREATE INDEX idx_notifications_comic_id ON USER_NOTIFICATION (UserID);
CREATE INDEX idx_notifications_genre_id ON USER_NOTIFICATION (ComicID);

CREATE INDEX idx_vote_comic_id ON USER_VOTE_COMIC (UserID);
CREATE INDEX idx_vote_genre_id ON USER_VOTE_COMIC (ComicID);


-- INSERT INTO hostcollector
-- (id,host,comic_format,chapter_format)
-- VALUES
-- (1,'nhattruyenss.com','https://{host}/truyen-tranh/{comic-slug}-{comicid}','https://{host}/truyen-tranh/{comic-slug}/{chapter-slug}/{chapterid}');




