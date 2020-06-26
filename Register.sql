create proc UserAdd
@Username nchar(20),
@Password nchar(20)
as
	insert into AUTH(Username,Password)
	values (@Username,@Password)

