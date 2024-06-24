
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
CREATE TRIGGER after_insert_chapter_notify
AFTER INSERT ON CHAPTER
FOR EACH ROW
EXECUTE FUNCTION notify_users_of_comic_update();


-- Trigger to remove notifications after deleting a chapter
CREATE TRIGGER after_delete_chapter_remove_notify
AFTER DELETE ON CHAPTER
FOR EACH ROW
EXECUTE FUNCTION remove_notifications_on_chapter_delete();

--END-----------------------------NOTIFY--------------------------------