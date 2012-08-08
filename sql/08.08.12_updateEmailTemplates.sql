UPDATE [PitchingTube].[dbo].[EmailTemplate]
   SET [Template] = '<div style="font-family: sans-serif"> <b>Hello #Name#</b>, <p> Your password at pitchingtube.com is <b>#Url#</b> </p> <br/></div>'
 WHERE [EmailTemplateID] = 'recoverpassword'
GO


