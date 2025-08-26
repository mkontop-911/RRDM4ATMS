-- To allow advanced options to be changed.
EXEC sp_configure 'show advanced options', 1;
GO
RECONFIGURE;
GO

-- To enable the feature.
EXEC sp_configure 'xp_cmdshell',1
GO
RECONFIGURE
GO