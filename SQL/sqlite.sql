CREATE TABLE _USER (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT UNIQUE,
    Email TEXT,
    HashPassword TEXT,
    Role INTEGER NOT NULL CHECK (Role IN (0, 1, 2))
);

CREATE TABLE COMIC (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT,
    URL TEXT NOT NULL UNIQUE,
    Author TEXT,
    Description TEXT,
    CoverImage TEXT,
    Status INTEGER NOT NULL CHECK (Status IN (0, 1)),
    Rating INTEGER NOT NULL CHECK (Rating <= 10),
    CreateAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdateAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE GENRE (
    ID INTEGER PRIMARY KEY,
    Title TEXT UNIQUE,
    Description TEXT
);

CREATE TABLE COMIC_GENRE (
    ComicID INTEGER,
    GenreID INTEGER,
    PRIMARY KEY (ComicID, GenreID),
    FOREIGN KEY (ComicID) REFERENCES COMIC(ID),
    FOREIGN KEY (GenreID) REFERENCES GENRE(ID)
);

CREATE TABLE CHAPTER (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    ComicID INTEGER,
    Title TEXT,
    URL TEXT,
    ChapterNumber INTEGER NOT NULL,
    Content TEXT,
    ViewCount INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (ComicID) REFERENCES COMIC(ID)
);
INSERT INTO GENRE (ID,Title, Description) VALUES
(1,'Actio', 'Thể loại này thường có nội dung về đánh nhau, bạo lực, hỗn loạn, với diễn biến nhanh'),
(2,'Adult', 'Thể loại có đề cập đến vấn đề nhạy cảm, chỉ dành cho tuổi 17+'),
(3,'Adventure', 'Thể loại phiêu lưu, mạo hiểm, thường là hành trình của các nhân vật'),
(4,'Anime', 'Truyện đã được chuyển thể thành film Anime'),
(5,'Chuyển Sinh', 'Thể loại này là những câu chuyện về người ở một thế giới này xuyên đến một thế giới khác, có thể là thế giới mang phong cách trung cổ với kiếm sĩ và ma thuật, hay thế giới trong game, hoặc có thể là bạn chết ở nơi này và được chuyển sinh đến nơi khác'),
(6,'Comedy', 'Thể loại có nội dung trong sáng và cảm động, thường có các tình tiết gây cười, các xung đột nhẹ nhàng'),
(7,'Comic', 'Truyện tranh Châu Âu và Châu Mĩ'),
(8,'Cooking', 'Thể loại có nội dung về nấu ăn, ẩm thực'),
(9,'Cổ Đại', 'Truyện có nội dung xảy ra ở thời cổ đại phong kiến.'),
(10,'Doujinshi', 'Thể loại truyện phóng tác do fan hay có thể cả những Mangaka khác với tác giả truyện gốc. Tác giả vẽ Doujinshi thường dựa trên những nhân vật gốc để viết ra những câu chuyện theo sở thích của mình'),
(11,'Drama', 'Thể loại mang đến cho người xem những cảm xúc khác nhau: buồn bã, căng thẳng thậm chí là bi phẫ'),
(12,'Đam Mỹ', 'Truyện tình cảm giữa nam và nam.'),
(13,'Ecchi', 'Thường có những tình huống nhạy cảm nhằm lôi cuốn người xem'),
(14,'Fantasy', 'Thể loại xuất phát từ trí tưởng tượng phong phú, từ pháp thuật đến thế giới trong mơ thậm chí là những câu chuyện thần tiê'),
(15,'Gender Bender', 'Là một thể loại trong đó giới tính của nhân vật bị lẫn lộn: nam hoá thành nữ, nữ hóa thành nam...'),
(16,'Harem', 'Thể loại truyện tình cảm, lãng mạn mà trong đó, nhiều nhân vật nữ thích một nam nhân vật chính'),
(17,'Historical', 'Thể loại có liên quan đến thời xa xưa'),
(18,'Horror', 'Horror là: rùng rợn, nghe cái tên là bạn đã hiểu thể loại này có nội dung thế nào. Nó làm cho bạn kinh hãi, khiếp sợ, ghê tởm, run rẩy, có thể gây sock - một thể loại không dành cho người yếu tim'),
(19,'Josei', 'Thể loại của manga hay anime được sáng tác chủ yếu bởi phụ nữ cho những độc giả nữ từ 18 đến 30. Josei manga có thể miêu tả những lãng mạn thực tế , nhưng trái ngược với hầu hết các kiểu lãng mạn lí tưởng của Shoujo manga với cốt truyện rõ ràng, chín chắ'),
(20,'Live actio', 'Truyện đã được chuyển thể thành phim'),
(21,'Manga', 'Truyện tranh của Nhật Bả'),
(22,'Manhua', 'Truyện tranh của Trung Quốc'),
(23,'Manhwa', 'Truyện tranh Hàn Quốc, đọc từ trái sang phải'),
(24,'Martial Arts', 'Giống với tên gọi, bất cứ gì liên quan đến võ thuật trong truyện từ các trận đánh nhau, tự vệ đến các môn võ thuật như akido, karate, judo hay taekwondo, kendo, các cách né tránh'),
(25,'Mature', 'Thể loại dành cho lứa tuổi 17+ bao gồm các pha bạo lực, máu me, chém giết, tình dục ở mức độ vừa'),
(26,'Mecha', 'Mecha, còn được biết đến dưới cái tên meka hay mechs, là thể loại nói tới những cỗ máy biết đi (thường là do phi công cầm lái)'),
(27,'Mystery', 'Thể loại thường xuất hiện những điều bí ấn không thể lí giải được và sau đó là những nỗ lực của nhân vật chính nhằm tìm ra câu trả lời thỏa đáng'),
(28,'Ngôn Tình', 'Truyện thuộc kiểu lãng mạn, kể về những sự kiện vui buồn trong tình yêu của nhân vật chính.'),
(29,'One shot', 'Những truyện ngắn, thường là 1 chapter'),
(30,'Psychological', 'Thể loại liên quan đến những vấn đề về tâm lý của nhân vật ( tâm thần bất ổn, điên cuồng ...),'),
(31,'Romance', 'Thường là những câu chuyện về tình yêu, tình cảm lãng mạn. Ở đây chúng ta sẽ lấy ví dụ như tình yêu giữa một người con trai và con gái, bên cạnh đó đặc điểm thể loại này là kích thích trí tưởng tượng của bạn về tình yêu'),
(32,'School Life', 'Trong thể loại này, ngữ cảnh diễn biến câu chuyện chủ yếu ở trường học'),
(33,'Sci-fi', 'Bao gồm những chuyện khoa học viễn tưởng, đa phần chúng xoay quanh nhiều hiện tượng mà liên quan tới khoa học, công nghệ, tuy vậy thường thì những câu chuyện đó không gắn bó chặt chẽ với các thành tựu khoa học hiện thời, mà là do con người tưởng tượng ra'),
(34,'Seine', 'Thể loại của manga thường nhằm vào những đối tượng nam 18 đến 30 tuổi, nhưng người xem có thể lớn tuổi hơn, với một vài bộ truyện nhắm đến các doanh nhân nam quá 40. Thể loại này có nhiều phong cách riêng biệt, nhưng thể loại này có những nét riêng biệt, thường được phân vào những phong cách nghệ thuật rộng hơn và phong phú hơn về chủ đề, có các loại từ mới mẻ tiên tiến đến khiêu dâm'),
(35,'Shoujo', 'Đối tượng hướng tới của thể loại này là phái nữ. Nội dung của những bộ manga này thường liên quan đến tình cảm lãng mạn, chú trọng đầu tư cho nhân vật (tính cách,...)'),
(36,'Shoujo Ai', 'Thể loại quan hệ hoặc liên quan tới đồng tính nữ, thể hiện trong các mối quan hệ trên mức bình thường giữa các nhân vật nữ trong các manga, anime'),
(37,'Shoune', 'Đối tượng hướng tới của thể loại này là phái nam. Nội dung của những bộ manga này thường liên quan đến đánh nhau và/hoặc bạo lực (ở mức bình thường, không thái quá)'),
(38,'Shounen Ai', 'Thể loại có nội dung về tình yêu giữa những chàng trai trẻ, mang tính chất lãng mạn nhưng không đề cập đến quan hệ tình dục'),
(39,'Slice of Life', 'Nói về cuộc sống đời thường'),
(40,'Smut', 'Những truyện có nội dung hơi nhạy cảm, đặc biệt là liên quan đến tình dục'),
(41,'Soft Yaoi', 'Boy x Boy. Nặng hơn Shounen Ai tí.'),
(42,'Soft Yuri', 'Girl x Girl. Nặng hơn Shoujo Ai tí'),
(43,'Sports', 'Đúng như tên gọi, những môn thể thao như bóng đá, bóng chày, bóng chuyền, đua xe, cầu lông,... là một phần của thể loại này'),
(44,'Supernatural', 'Thể hiện những sức mạnh đáng kinh ngạc và không thể giải thích được, chúng thường đi kèm với những sự kiện trái ngược hoặc thách thức với những định luật vật lý'),
(45,'Thiếu Nhi', 'Truyện tranh dành cho lứa tuổi thiếu nhi'),
(46,'Tragedy', 'Thể loại chứa đựng những sự kiện mà dẫn đến kết cục là những mất mát hay sự rủi ro to lớ'),
(47,'Trinh Thám', 'Các truyện có nội dung về các vụ án, các thám tử cảnh sát điều tra...'),
(48,'Truyện sca', 'Các truyện đã phát hành tại VN được scan đăng online'),
(49,'Truyện Màu', 'Tổng hợp truyện tranh màu, rõ, đẹp'),
(50,'Webtoo', 'Là truyện tranh được đăng dài kỳ trên internet của Hàn Quốc chứ không xuất bản theo cách thông thường'),
(51,'Xuyên Không', 'Xuyên Không, Xuyên Việt là thể loại nhân vật chính vì một lý do nào đó mà bị đưa đến sinh sống ở một không gian hay một khoảng thời gian khác. Nhân vật chính có thể trực tiếp xuyên qua bằng thân xác mình hoặc sống lại bằng thân xác người khác.')
