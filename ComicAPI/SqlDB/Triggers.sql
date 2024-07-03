
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
            'Truyện "' || comic.Title || '" ra chap mới: ' || NEW.Title,
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




--END-----------------------------NOTIFY--------------------------------


--BEGIN-------------------------------Rating--------------------------------

CREATE OR REPLACE FUNCTION update_comic_rating()
RETURNS TRIGGER AS $$
DECLARE
    avg_rating NUMERIC;
BEGIN
    -- Tính điểm trung bình rating cho comic và làm tròn tới thập phân thứ nhất
    SELECT ROUND(AVG(VotePoint), 1) INTO avg_rating
    FROM USER_VOTE_COMIC
    WHERE ComicID = COALESCE(NEW.ComicID, OLD.ComicID);

    -- Cập nhật điểm trung bình vào bảng COMIC
    UPDATE COMIC
    SET Rating = COALESCE(avg_rating, 0)
    WHERE ID = COALESCE(NEW.ComicID, OLD.ComicID);

    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

-- Trigger sau khi insert hoặc update hoặc delete bảng USER_VOTE_COMIC
CREATE OR REPLACE TRIGGER update_comic_rating
AFTER INSERT OR UPDATE OR DELETE ON USER_VOTE_COMIC
FOR EACH ROW
EXECUTE FUNCTION update_comic_rating();

--END-------------------------------Rating--------------------------------