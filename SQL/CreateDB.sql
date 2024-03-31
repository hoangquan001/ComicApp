--Tên: Hoàng Anh Quân 
--MSSV: 19120628
--DROP DATABASE ComicApp
CREATE DATABASE ComicApp
GO
USE ComicApp
GO

CREATE TABLE Users (
    id INT NOT NULL IDENTITY(1,1),
    username NVARCHAR(50) UNIQUE,
    email NVARCHAR(100),
    hash_password NVARCHAR(255),
	CONSTRAINT PK_User PRIMARY KEY (id)
);


CREATE TABLE Comics (
    id INT NOT NULL IDENTITY(1,1),
    title NVARCHAR(255),
    author NVARCHAR(100),
    description NVARCHAR(MAX),
    cover_image NVARCHAR(255),
    CONSTRAINT PK_Comics PRIMARY KEY (id)
);

CREATE TABLE Genres (
    id INT NOT NULL IDENTITY(1,1),
    title NVARCHAR(255),
    description NVARCHAR(MAX),
    CONSTRAINT PK_Genres PRIMARY KEY (id)
);


CREATE TABLE Comic_Genre (
    comicid INT,
    genreid INT,
    CONSTRAINT PK_Comic_Genre PRIMARY KEY (comicid, genreid),
	CONSTRAINT FK_Comic_Genre_comic FOREIGN KEY (comicid) REFERENCES Comics(id),
	CONSTRAINT FK_Comic_Genre_genre FOREIGN KEY (genreid) REFERENCES Genres(id)
);



CREATE TABLE Chapters (
    id INT NOT NULL IDENTITY(1,1),
    comicid INT,
    chapter_number INT,
    title NVARCHAR(255),
    content NVARCHAR(MAX),
    CONSTRAINT FK_Chapters PRIMARY KEY (id),
    FOREIGN KEY (comicid) REFERENCES Comics(id)
);


INSERT INTO Comics (title, author, description, cover_image) VALUES
('Comic 1', 'Author 1', 'Description for Comic 1', '/images/comic1.jpg'),
('Comic 2', 'Author 2', 'Description for Comic 2', '/images/comic2.jpg'),
('Comic 3', 'Author 3', 'Description for Comic 3', '/images/comic3.jpg'),
('Comic 4', 'Author 4', 'Description for Comic 4', '/images/comic4.jpg'),
('Comic 5', 'Author 5', 'Description for Comic 5', '/images/comic5.jpg'),
('Comic 6', 'Author 6', 'Description for Comic 6', '/images/comic6.jpg'),
('Comic 7', 'Author 7', 'Description for Comic 7', '/images/comic7.jpg'),
('Comic 8', 'Author 8', 'Description for Comic 8', '/images/comic8.jpg'),
('Comic 9', 'Author 9', 'Description for Comic 9', '/images/comic9.jpg'),
('Comic 10', 'Author 10', 'Description for Comic 10', '/images/comic10.jpg');

INSERT INTO Genres (title, description) VALUES
(N'Action', N'Thể loại này thường có nội dung về đánh nhau, bạo lực, hỗn loạn, với diễn biến nhanh'),
(N'Adult', N'Thể loại có đề cập đến vấn đề nhạy cảm, chỉ dành cho tuổi 17+'),
(N'Adventure', N'Thể loại phiêu lưu, mạo hiểm, thường là hành trình của các nhân vật'),
(N'Anime', N'Truyện đã được chuyển thể thành film Anime'),
(N'Chuyển Sinh', N'Thể loại này là những câu chuyện về người ở một thế giới này xuyên đến một thế giới khác, có thể là thế giới mang phong cách trung cổ với kiếm sĩ và ma thuật, hay thế giới trong game, hoặc có thể là bạn chết ở nơi này và được chuyển sinh đến nơi khác'),
(N'Comedy', N'Thể loại có nội dung trong sáng và cảm động, thường có các tình tiết gây cười, các xung đột nhẹ nhàng'),
(N'Comic', N'Truyện tranh Châu Âu và Châu Mĩ'),
(N'Cooking', N'Thể loại có nội dung về nấu ăn, ẩm thực'),
(N'Cổ Đại', N'Truyện có nội dung xảy ra ở thời cổ đại phong kiến.'),
(N'Doujinshi', N'Thể loại truyện phóng tác do fan hay có thể cả những Mangaka khác với tác giả truyện gốc. Tác giả vẽ Doujinshi thường dựa trên những nhân vật gốc để viết ra những câu chuyện theo sở thích của mình'),
(N'Drama', N'Thể loại mang đến cho người xem những cảm xúc khác nhau: buồn bã, căng thẳng thậm chí là bi phẫn'),
(N'Đam Mỹ', N'Truyện tình cảm giữa nam và nam.'),
(N'Ecchi', N'Thường có những tình huống nhạy cảm nhằm lôi cuốn người xem'),
(N'Fantasy', N'Thể loại xuất phát từ trí tưởng tượng phong phú, từ pháp thuật đến thế giới trong mơ thậm chí là những câu chuyện thần tiên'),
(N'Gender Bender', N'Là một thể loại trong đó giới tính của nhân vật bị lẫn lộn: nam hoá thành nữ, nữ hóa thành nam...'),
(N'Harem', N'Thể loại truyện tình cảm, lãng mạn mà trong đó, nhiều nhân vật nữ thích một nam nhân vật chính'),
(N'Historical', N'Thể loại có liên quan đến thời xa xưa'),
(N'Horror', N'Horror là: rùng rợn, nghe cái tên là bạn đã hiểu thể loại này có nội dung thế nào. Nó làm cho bạn kinh hãi, khiếp sợ, ghê tởm, run rẩy, có thể gây sock - một thể loại không dành cho người yếu tim'),
(N'Josei', N'Thể loại của manga hay anime được sáng tác chủ yếu bởi phụ nữ cho những độc giả nữ từ 18 đến 30. Josei manga có thể miêu tả những lãng mạn thực tế , nhưng trái ngược với hầu hết các kiểu lãng mạn lí tưởng của Shoujo manga với cốt truyện rõ ràng, chín chắn'),
(N'Live action', N'Truyện đã được chuyển thể thành phim'),
(N'Manga', N'Truyện tranh của Nhật Bản'),
(N'Manhua', N'Truyện tranh của Trung Quốc'),
(N'Manhwa', N'Truyện tranh Hàn Quốc, đọc từ trái sang phải'),
(N'Martial Arts', N'Giống với tên gọi, bất cứ gì liên quan đến võ thuật trong truyện từ các trận đánh nhau, tự vệ đến các môn võ thuật như akido, karate, judo hay taekwondo, kendo, các cách né tránh'),
(N'Mature', N'Thể loại dành cho lứa tuổi 17+ bao gồm các pha bạo lực, máu me, chém giết, tình dục ở mức độ vừa'),
(N'Mecha', N'Mecha, còn được biết đến dưới cái tên meka hay mechs, là thể loại nói tới những cỗ máy biết đi (thường là do phi công cầm lái)'),
(N'Mystery', N'Thể loại thường xuất hiện những điều bí ấn không thể lí giải được và sau đó là những nỗ lực của nhân vật chính nhằm tìm ra câu trả lời thỏa đáng'),
(N'Ngôn Tình', N'Truyện thuộc kiểu lãng mạn, kể về những sự kiện vui buồn trong tình yêu của nhân vật chính.'),
(N'One shot', N'Những truyện ngắn, thường là 1 chapter'),
(N'Psychological', N'Thể loại liên quan đến những vấn đề về tâm lý của nhân vật ( tâm thần bất ổn, điên cuồng ...),'),
(N'Romance', N'Thường là những câu chuyện về tình yêu, tình cảm lãng mạn. Ở đây chúng ta sẽ lấy ví dụ như tình yêu giữa một người con trai và con gái, bên cạnh đó đặc điểm thể loại này là kích thích trí tưởng tượng của bạn về tình yêu'),
(N'School Life', N'Trong thể loại này, ngữ cảnh diễn biến câu chuyện chủ yếu ở trường học'),
(N'Sci-fi', N'Bao gồm những chuyện khoa học viễn tưởng, đa phần chúng xoay quanh nhiều hiện tượng mà liên quan tới khoa học, công nghệ, tuy vậy thường thì những câu chuyện đó không gắn bó chặt chẽ với các thành tựu khoa học hiện thời, mà là do con người tưởng tượng ra'),
(N'Seinen', N'Thể loại của manga thường nhằm vào những đối tượng nam 18 đến 30 tuổi, nhưng người xem có thể lớn tuổi hơn, với một vài bộ truyện nhắm đến các doanh nhân nam quá 40. Thể loại này có nhiều phong cách riêng biệt, nhưng thể loại này có những nét riêng biệt, thường được phân vào những phong cách nghệ thuật rộng hơn và phong phú hơn về chủ đề, có các loại từ mới mẻ tiên tiến đến khiêu dâm'),
(N'Shoujo', N'Đối tượng hướng tới của thể loại này là phái nữ. Nội dung của những bộ manga này thường liên quan đến tình cảm lãng mạn, chú trọng đầu tư cho nhân vật (tính cách,...)'),
(N'Shoujo Ai', N'Thể loại quan hệ hoặc liên quan tới đồng tính nữ, thể hiện trong các mối quan hệ trên mức bình thường giữa các nhân vật nữ trong các manga, anime'),
(N'Shounen', N'Đối tượng hướng tới của thể loại này là phái nam. Nội dung của những bộ manga này thường liên quan đến đánh nhau và/hoặc bạo lực (ở mức bình thường, không thái quá)'),
(N'Shounen Ai', N'Thể loại có nội dung về tình yêu giữa những chàng trai trẻ, mang tính chất lãng mạn nhưng không đề cập đến quan hệ tình dục'),
(N'Slice of Life', N'Nói về cuộc sống đời thường'),
(N'Smut', N'Những truyện có nội dung hơi nhạy cảm, đặc biệt là liên quan đến tình dục'),
(N'Soft Yaoi', N'Boy x Boy. Nặng hơn Shounen Ai tí.'),
(N'Soft Yuri', N'Girl x Girl. Nặng hơn Shoujo Ai tí'),
(N'Sports', N'Đúng như tên gọi, những môn thể thao như bóng đá, bóng chày, bóng chuyền, đua xe, cầu lông,... là một phần của thể loại này'),
(N'Supernatural', N'Thể hiện những sức mạnh đáng kinh ngạc và không thể giải thích được, chúng thường đi kèm với những sự kiện trái ngược hoặc thách thức với những định luật vật lý'),
(N'Thiếu Nhi', N'Truyện tranh dành cho lứa tuổi thiếu nhi'),
(N'Tragedy', N'Thể loại chứa đựng những sự kiện mà dẫn đến kết cục là những mất mát hay sự rủi ro to lớn'),
(N'Trinh Thám', N'Các truyện có nội dung về các vụ án, các thám tử cảnh sát điều tra...'),
(N'Truyện scan', N'Các truyện đã phát hành tại VN được scan đăng online'),
(N'Truyện Màu', N'Tổng hợp truyện tranh màu, rõ, đẹp'),
(N'Webtoon', N'Là truyện tranh được đăng dài kỳ trên internet của Hàn Quốc chứ không xuất bản theo cách thông thường'),
(N'Xuyên Không', N'Xuyên Không, Xuyên Việt là thể loại nhân vật chính vì một lý do nào đó mà bị đưa đến sinh sống ở một không gian hay một khoảng thời gian khác. Nhân vật chính có thể trực tiếp xuyên qua bằng thân xác mình hoặc sống lại bằng thân xác người khác.')

INSERT INTO Comic_Genre (genreid, comicid) VALUES
(52,1),
(52,2),
(52,3),
(52,4),
(52,5),
(52,6),
(52,7),
(52,8),
(53,9),
(53,10),
(54,1),
(54,2)

INSERT INTO Chapters (comicid, title, content, chapter_number) VALUES
(1, N'Chương 1', N'Noi dung chuong 1', 1),
(1, N'Chuong 2', N'Noi dung chuong 2', 2),
(1, N'Chuong 3', N'Noi dung chuong 3', 3),
(1, N'Chuong 4', N'Noi dung chuong 4', 4),
(1, N'Chuong 5', N'Noi dung chuong 5', 5),
(1, N'Chuong 6', N'Noi dung chuong 6', 6),
(1, N'Chuong 7', N'Noi dung chuong 7', 7),
(1, N'Chuong 8', N'Noi dung chuong 8', 8),
(1, N'Chuong 9', N'Noi dung chuong 9', 9),
(1, N'Chuong 10', N'Noi dung chuong 10', 10),
(1, N'Chuong 11', N'Noi dung chuong 11', 11)


SELECT * 
FROM Users
SELECT * 
FROM Comics
SELECT * 
FROM Comic_Genre
SELECT * 
FROM Chapters
SELECT * 
FROM Genres

--DELETE FROM Genres WHERE id>0;