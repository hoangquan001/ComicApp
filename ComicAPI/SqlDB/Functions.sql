-----GetLastChapter------
CREATE OR REPLACE FUNCTION get_latest_chapter(comic_id INT)
RETURNS TABLE (
    ID INT,
    ComicID INT,
    Title VARCHAR,
    ChapterNumber INT,
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
    ORDER BY ChapterNumber DESC
    LIMIT 1;
END;
$$ LANGUAGE plpgsql;

----Update View

CREATE OR REPLACE FUNCTION update_view()
RETURNS void
AS $$
BEGIN
    UPdate comic  set viewcount = v.view 
    from 
    (	
        select comicid, sum(ct.viewcount) as view
        from  chapter ct 
        group by comicid
    )as v
    where v.comicid = id
END;
$$ LANGUAGE plpgsql;

--Synchronize comic
CREATE OR REPLACE FUNCTION synchronize_comic()
RETURNS void
AS $$
BEGIN
    -- Update the UpdateAt and numchapter fields
    UPDATE COMIC
    SET updateAt = lc.UpdateAt,
        numchapter = lc.chapterNumber
    from get_latest_chapter(NEW.ComicID) as lc 
    WHERE Comic.id = NEW.ComicID;
END;
$$ LANGUAGE plpgsql;


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