create proc UserAdd
@Username nchar(10),
@Password nchar(10)
as
	insert into AUTH(Username,Password)
	values (@Username,@Password)

