-- ResetChapterID
SELECT SETVAL('chapter_id_seq',1500000);
-----GetLastChapter------
CREATE OR REPLACE FUNCTION get_latest_chapter(comic_id INT)
RETURNS TABLE (
    ID INT,
    ComicID INT,
    Title VARCHAR,
    URL VARCHAR,
    ViewCount INT,
    UpdateAt TIMESTAMP,
    Pages TEXT
) AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM CHAPTER
    WHERE CHAPTER.ComicID = comic_id
    ORDER BY CHAPTER.url::float DESC
    LIMIT 1;
END;
$$ LANGUAGE plpgsql;

----------------------BEGIN COMIC------------------------------
-- Synchronize comic : set static data of chapter to comic
CREATE OR REPLACE FUNCTION synchronize_comic()
RETURNS void
AS $$
BEGIN

	UPdate comic  set viewcount = v.view , numchapter = v.num
	from 
	(	
		select comicid, sum(ct.viewcount) as view , count(id) as num
		from  chapter ct 
		group by comicid
	)as v
	where v.comicid = id;


    Update comic
    SET lastchapter = lastchapter.id,
        updateat = lastchapter.updateat
    FROM (
        Select * from chapter where comicid = chapter.comicid
        ORDER BY url::float DESC
        LIMIT 1
    ) as lastchapter
    WHERE comic.id = lastchapter.comicid;
END;
$$ LANGUAGE plpgsql;
----------------------END COMIC------------------------------

--BEGIN-------------------------------NOTIFY--------------------------------
-- Function to delete notifications when a chapter is deleted

-- Function to create notifications for users following a comic
CREATE OR REPLACE FUNCTION notify_users_of_comic_update()
RETURNS TRIGGER AS $$
DECLARE
    user RECORD;
BEGIN
    FOR user IN
        SELECT UserID
        FROM USER_FOLLOW_COMIC
        WHERE ComicID = NEW.ComicID
    LOOP
        INSERT INTO NOTIFICATIONS (UserID, ComicID, NotificationContent)
        VALUES (
            user.UserID,
            NEW.ComicID,
            'The comic "' || (SELECT Title FROM COMIC WHERE ID = NEW.ComicID) || '" has a new chapter: ' || NEW.Title
        );
    END LOOP;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;




--END-----------------------------NOTIFY--------------------------------