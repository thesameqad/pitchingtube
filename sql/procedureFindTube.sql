USE [PitchingTube]
GO

/****** Object:  StoredProcedure [dbo].[findTube]    Script Date: 08/20/2012 15:37:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Batch submitted through debugger: SQLQuery2.sql|4|0|C:\Users\1\AppData\Local\Temp\~vsE766.sql
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[findTube]
	-- Add the parameters for the stored procedure here
	@UserRole nvarchar(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select t.TubeId, COUNT(k.UserId)
	from Tubes as t
		left join 
		(
			select p.TubeId, u.UserId
			from Participants as p
				join aspnet_Users as u on p.UserId = u.UserId
					join aspnet_UsersInRoles as ur on u.UserId = ur.UserId
						join aspnet_Roles as r on ur.RoleId = r.RoleId
			where r.RoleName = @UserRole
		) as k on t.TubeId = k.TubeId
	--where t.Mode = 0
	group by t.TubeId
	having COUNT(k.UserId) < 5
	order by COUNT(k.UserId) desc
END

GO


