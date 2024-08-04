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



--BEGIN---------------------------TOPVIEW--------------------------------

CREATE OR REPLACE FUNCTION get_top_daily_comics()
RETURNS TABLE (ComicID INT) AS $$
BEGIN
    RETURN QUERY
    WITH top_daily AS (
        SELECT dc.ComicID, SUM(dc.ViewCount) AS TotalViewCount
        FROM DAILY_COMIC_VIEWS dc
        WHERE  DATE(dc.ViewDate) = DATE(CURRENT_DATE)
        GROUP BY dc.ComicID
        ORDER BY TotalViewCount DESC
        
    ),
    additional_comics AS (
        SELECT c.ID AS ComicID
        FROM COMIC c
        WHERE c.ID NOT IN (SELECT td.ComicID FROM top_daily td)
        ORDER BY c.ViewCount DESC
         
    )
    SELECT td.ComicID
    FROM top_daily td
    UNION ALL
    SELECT ac.ComicID
    FROM additional_comics ac;
END;
$$ LANGUAGE plpgsql

CREATE OR REPLACE FUNCTION get_top_weekly_comics()
RETURNS TABLE (ComicID INT) AS $$
BEGIN
    RETURN QUERY
    WITH top_weekly AS (
        SELECT dc.ComicID, SUM(dc.ViewCount) AS TotalViewCount
        FROM DAILY_COMIC_VIEWS dc
        WHERE dc.ViewDate >= CURRENT_DATE - INTERVAL '7 days'
        GROUP BY dc.ComicID
        ORDER BY TotalViewCount DESC
    ),
    additional_comics AS (
        SELECT c.ID AS ComicID
        FROM COMIC c
        WHERE c.ID NOT IN (SELECT tw.ComicID FROM top_weekly tw)
        ORDER BY c.ViewCount DESC
    )
    SELECT tw.ComicID
    FROM top_weekly tw
    UNION ALL
    SELECT ac.ComicID
    FROM additional_comics ac;
END;
$$ LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION get_top_monthly_comics()
RETURNS TABLE (ComicID INT) AS $$
BEGIN
    RETURN QUERY
    WITH top_monthly AS (
        SELECT dc.ComicID, SUM(dc.ViewCount) AS TotalViewCount
        FROM DAILY_COMIC_VIEWS dc
        WHERE dc.ViewDate >= CURRENT_DATE - INTERVAL '1 month'
        GROUP BY dc.ComicID
        ORDER BY TotalViewCount DESC
        
    ),
    additional_comics AS (
        SELECT c.ID AS ComicID
        FROM COMIC c
        WHERE c.ID NOT IN (SELECT tm.ComicID FROM top_monthly tm)
        ORDER BY c.ViewCount DESC
    )
    SELECT tm.ComicID
    FROM top_monthly tm
    UNION ALL
    SELECT ac.ComicID
    FROM additional_comics ac;
END;
$$ LANGUAGE plpgsql;



--BEGIN---------------------------END--------------------------------