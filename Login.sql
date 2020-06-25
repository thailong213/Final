/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [IDuser]
      ,[Username]
      ,[Password]
  FROM [User].[dbo].[AUTH]

