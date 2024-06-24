
--BEGIN-------------------------------CHAPTER--------------------------------
-- TRIGGER WHEN INSERT CHAPTER
CREATE OR REPLACE FUNCTION update_comic_after_insert_chapters()
RETURNS TRIGGER AS $$
BEGIN
    -- Update the UpdateAt and numchapter fields
    UPDATE COMIC
    SET updateAt = lc.UpdateAt,
        numchapter = lc.chapterNumber,
        lastChapter = NEW.ID
    from get_latest_chapter(NEW.ComicID) as lc 
    WHERE Comic.id = NEW.ComicID;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER after_insert_chapters
AFTER INSERT OR DELETE ON CHAPTER
FOR EACH ROW
EXECUTE FUNCTION update_comic_after_insert_chapters();


-- TRIGGER WHEN DELETE CHAPTER
CREATE OR REPLACE FUNCTION update_comic_after_delete_chapters()
RETURNS TRIGGER AS $$
BEGIN
    -- Update the UpdateAt and numchapter fields
    UPDATE COMIC
    SET 
        updateAt = lc.UpdateAt,
        numchapter = lc.chapterNumber,
        lastChapter = CASE WHEN lc.ID = OLD.ID THEN NULL ELSE lc.ID END
    FROM get_latest_chapter(OLD.ComicID) AS lc 
    WHERE COMIC.ID = OLD.ComicID;

    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER after_delete_chapters
AFTER DELETE ON CHAPTER
FOR EACH ROW
EXECUTE FUNCTION update_comic_after_delete_chapters();

--END-----------------------------CHAPTER--------------------------------


--BEGIN-------------------------------NOTIFY--------------------------------
-- Trigger to notify users after inserting a new chapter
CREATE OR REPLACE FUNCTION notify_users_of_comic_update()
RETURNS TRIGGER AS $$
DECLARE
    user_id INTEGER;
    comic RECORD;
BEGIN
    -- Fetch comic details
    SELECT Title, URLComic, CoverImage INTO comic
    FROM COMIC
    WHERE ID = NEW.ComicID;

    FOR user_id IN
        SELECT UserID
        FROM USER_FOLLOW_COMIC
        WHERE ComicID = NEW.ComicID
    LOOP
        INSERT INTO USER_NOTIFICATION (UserID, ComicID, NotificationContent, CoverImage, URLComic, lastchapter, URLChapter)
        VALUES (
            user_id,
            NEW.ComicID,
            'The comic "' || comic.Title || '" has a new chapter: ' || NEW.Title,
            comic.CoverImage,
            comic.URLComic,
            NEW.ID,
            NEW.URL
        );
    END LOOP;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION remove_notifications_on_chapter_delete()
RETURNS TRIGGER AS $$
BEGIN
    DELETE FROM USER_NOTIFICATION
    WHERE ComicID = OLD.ComicID
      AND NotificationContent LIKE '%' || OLD.Title || '%';

    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER after_insert_chapter_notify
AFTER INSERT ON CHAPTER
FOR EACH ROW
EXECUTE FUNCTION notify_users_of_comic_update();


-- Trigger to remove notifications after deleting a chapter

CREATE OR REPLACE FUNCTION remove_notifications_on_chapter_delete()
RETURNS TRIGGER AS $$
BEGIN
    DELETE FROM USER_NOTIFICATION
    WHERE ComicID = OLD.ComicID
      AND NotificationContent LIKE '%' || OLD.Title || '%';

    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER after_delete_chapter_remove_notify
AFTER DELETE ON CHAPTER
FOR EACH ROW
EXECUTE FUNCTION remove_notifications_on_chapter_delete();

--END-----------------------------NOTIFY--------------------------------