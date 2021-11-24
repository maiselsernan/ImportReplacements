namespace ImportReplacement.Api
{
    public static class Queries
    {
        internal const string SaveApprovedMaintainsQuery = @"DECLARE @approved Table(
	[command_id] [bigint] NOT NULL,
	[file] [varchar](255) NULL,
	[meter_id] [bigint] NULL,
	[address] [varchar](255) NULL,
	[user] [int] NULL,
	[action] [smallint] NULL,
	[datetime] [datetime] NULL,
	[longitude] [float] NULL,
	[latitude] [float] NULL,
	[old_number] [bigint] NULL,
	[old_reading] [int] NULL,
	[old_image] [nvarchar](MAX) NULL,
	[new_number] [bigint] NULL,
	[reading] [int] NULL,
	[new_image] [nvarchar](MAX) NULL,
	[comment] [text] NULL,
	[code] [bigint] NULL,
	[code_description] [text] NULL,
	[diameter] [int] NULL,
	[replacement_type] [int] NULL,
	[consumer_id] [bigint] NULL,
	[approved] [bit] NOT NULL,
	[handled] [bit] NULL,
	[channel_id] [bigint] NULL,
	[meter_model] [int] NULL,
	[meter_manufacturer] [int] NULL,
	[meter_type] [int] NULL
);

UPDATE	replacement.ReplacementModel
SET		handled = IIF([channel_id] IS NULL, 0, 1),
		approved = IIF([channel_id] IS NULL, 0, approved),
		replacement_type = IIF([channel_id] IS NULL, 6, replacement_type)
OUTPUT  INSERTED.[command_id]
           ,INSERTED.[file]
           ,INSERTED.[meter_id]
           ,INSERTED.[address]
           ,INSERTED.[user]
           ,INSERTED.[action]
           ,INSERTED.[datetime]
           ,INSERTED.[longitude]
           ,INSERTED.[latitude]
           ,INSERTED.[old_number]
           ,INSERTED.[old_reading]
           ,INSERTED.[old_image]
           ,INSERTED.[new_number]
           ,INSERTED.[reading]
           ,INSERTED.[new_image]
           ,INSERTED.[comment]
           ,INSERTED.[code]
           ,INSERTED.[code_description]
           ,INSERTED.[diameter]
           ,INSERTED.[replacement_type]
           ,INSERTED.[consumer_id]
           ,INSERTED.[approved]
           ,INSERTED.[handled]
           ,INSERTED.[channel_id]
           ,INSERTED.[meter_model]
           ,INSERTED.[meter_manufacturer]
           ,INSERTED.[meter_type]
INTO @approved([command_id]
           ,[file]
           ,[meter_id]
           ,[address]
           ,[user]
           ,[action]
           ,[datetime]
           ,[longitude]
           ,[latitude]
           ,[old_number]
           ,[old_reading]
           ,[old_image]
           ,[new_number]
           ,[reading]
           ,[new_image]
           ,[comment]
           ,[code]
           ,[code_description]
           ,[diameter]
           ,[replacement_type]
           ,[consumer_id]
           ,[approved]
           ,[handled]
           ,[channel_id]
           ,[meter_model]
           ,[meter_manufacturer]
           ,[meter_type])
WHERE  handled = 0 AND approved = 1 AND replacement_type = 1 AND (site_id = @SiteId OR @SiteId = 0);

UPDATE  up
SET     ToDate = approved.[datetime]
FROM    ConsumerChannelHistory up
        JOIN @approved AS approved ON approved.consumer_id = up.Consumer
WHERE   up.ToDate IS NULL;

INSERT INTO ConsumerChannelHistory(Consumer, Channel,FromDate,ToDate,UserID,MeterNumber,Diameter,meterManufacturer,meterModel,meterType,OldImage,NewImage,CommandID)
SELECT consumer_id, channel_id, [datetime], NULL, -1, new_number, diameter, meter_manufacturer,meter_model,meter_type, 
       IIF(old_image LIKE '/%', CONCAT('madr:', SUBSTRING(old_image, 2, LEN(old_image) - 1)), old_image),
       IIF(new_image LIKE '/%', CONCAT('madr:', SUBSTRING(new_image, 2, LEN(new_image) - 1)), new_image),
	   command_id
FROM   @approved AS approved
WHERE  channel_id IS NOT NULL;

UPDATE	up
SET		Channel = 543,
		MeterNumber = approved.new_number,
		Diameter = approved.diameter,
		meterManufacturer = approved.meter_manufacturer,
		meterModel = approved.meter_model,
		meterType = approved.meter_type
FROM	Consumers AS up
		JOIN @approved AS approved ON approved.consumer_id = up.id
WHERE	channel_id IS NULL;

SELECT (Payer.FirstName  + ' ' +  Payer.LastName) AS FullName,  meter_id AS MeterId,	address AS Address,	datetime AS DateTime,	longitude AS Longitude,	latitude AS Latitude,	old_number AS OldNumber,
        old_reading AS OldReading,	IIF(LEN(old_image) > 0,'=HYPERLINK(""http://reader.madrimonim.co.il/reading_images' +  old_image +  '"")',NULL) AS OldImage,	new_number AS NewNumber,	
        reading AS Reading,	IIF(LEN(old_image) > 0,'=HYPERLINK(""http://reader.madrimonim.co.il/reading_images' + new_image + '"")', NULL) AS NewImage,
        DiameterTypes.inch AS Diameter, comment AS Comment
FROM    @approved as approved
		LEFT JOIN Consumers on Consumers.ID = approved.consumer_id
		LEFT JOIN Payer on Payer.ID = Consumers.PayerID
        LEFT JOIN DiameterTypes ON DiameterTypes.ID = approved.diameter";
        internal const string UpdateRowQuery = @"UPDATE	replacement.ReplacementModel
SET		reading = @Reading,
		old_reading = @OldReading,
        approved = @Approved,
        meter_type = @MeterType,
        meter_manufacturer = @MeterManufacturer,
        meter_model = @MeterModel,
        channel_id = @ChannelId,
        old_number = @OldNumber,
        new_number = @NewNumber,
        Billable = @Billable,
        ReasonId = @ReasonId,
        ChargeDescription = @ChargeDescription
WHERE	command_id = @CommandId
        AND EXISTS (SELECT consumer_id INTERSECT SELECT @Consumer)
        AND EXISTS (SELECT NonAmrConsumerId INTERSECT SELECT @NonAmrConsumer)
        AND site_id = @SiteID";

        internal const string UpdateChargeRowQuery = @"UPDATE replacement.ReplacementModel
SET	Charged = @Charged   
WHERE	command_id = @CommandId
        AND EXISTS (SELECT consumer_id INTERSECT SELECT @ConsumerId)
        AND site_id = @SiteID";

        internal const string RowsToMaintainQuery = @"SELECT command_id AS	CommandId, old_number AS OldNumber, new_number AS NewNumber, AddressIndex.Name AS AddressLine1,
		Property.AddressLine2, Payer.FirstName, Payer.LastName, ReplacementModel.site_id AS SiteID, Sites.SiteName, approved AS Approved,
        channel_id AS ChannelId, meter_model AS MeterModel, meter_manufacturer AS MeterManufacturer, meter_type AS MeterType,
		Consumers.ID AS Consumer,
        ReplacementModel.NonAmrConsumerId AS NonAmrConsumer,
        DiameterTypes.Name AS Diameter,
        replacement.ReplacementModel.old_image AS OldImage, replacement.ReplacementModel.new_image AS NewImage, replacement.ReplacementModel.code AS Code,
        replacement.ReplacementModel.comment AS Comment, Channels.MeterNumber AS ChannelMeterNumber, longitude, latitude, 
        replacement.ReplacementModel.Billable,replacement.ReplacementModel.ReasonId, replacement.ReplacementModel.ChargeDescription, ReplacementModel.datetime AS ReplaceDate, Consumers.Expired, [user]
        ,ReplacementModel.reading, ReplacementModel.old_reading AS OldReading
FROM replacement.ReplacementModel
	 JOIN Consumers ON Consumers.ID = ReplacementModel.consumer_id 
	 LEFT JOIN Sites ON Sites.ID = ReplacementModel.site_id
	 JOIN Property ON Property.ID = Consumers.Property
	 JOIN AddressIndex ON AddressIndex.ID = Property.AddressLine1
	 JOIN Payer ON Payer.ID = Consumers.PayerID
     LEFT JOIN Channels ON Channels.ID = replacement.ReplacementModel.channel_id
     LEFT JOIN DiameterTypes ON DiameterTypes.ID = replacement.ReplacementModel.diameter
WHERE handled = 0
	  AND  replacement_type IN @ReplacementTypes";
        internal const string UndefinedRowsQuery = @"SELECT command_id, [file], meter_id, [address], [user], [datetime],
                    longitude, latitude, old_number, old_image, new_number, new_image, comment, code, code_description, 
                    DiameterTypes.Name AS Diameter ,site_id, Billable, ReasonId, ChargeDescription,
	ISNULL((
		SELECT Consumers.SiteID AS SiteId, Sites.SiteName , Consumers.ID AS Id, Property.AddressLine2 AS Address, Payer.FirstName, Payer.LastName
		FROM Consumers JOIN Property ON Property.ID = Consumers.Property
		JOIN Payer ON Payer.ID = Consumers.PayerID
		LEFT JOIN Sites ON Sites.ID = Consumers.SiteID
		WHERE Consumers.MeterNumber = ReplacementModel.new_number AND Consumers.SiteID <> 29 
		FOR JSON PATH
	),'[]') AS ExistingConsumers
 FROM replacement.ReplacementModel
 LEFT JOIN DiameterTypes ON DiameterTypes.ID = replacement.ReplacementModel.diameter
 WHERE handled=0 AND replacement_type=@Type ";
        
        internal const string RowsToInstallQuery = @"SELECT	command_id AS CommandId, old_number AS OldNumber, new_number AS NewNumber,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, '', NonAmrConsumers.AddressLine1) AddressLine1,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, ReplacementModel.Address, NonAmrConsumers.AddressLine2) AddressLine2,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, ReplacementModel.account_name, NonAmrConsumers.FirstName) FirstName,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, '', NonAmrConsumers.LastName) LastName,
        ReplacementModel.site_id AS SiteID, Sites.SiteName, approved AS Approved,
        channel_id AS ChannelId, meter_model AS MeterModel, meter_manufacturer AS MeterManufacturer, meter_type AS MeterType,
        ReplacementModel.consumer_id AS Consumer,
        ReplacementModel.NonAmrConsumerId AS NonAmrConsumer,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, ISNULL(DiameterTypes.Name, '3/4""'), DiameterTypes.Name) Diameter,
        replacement.ReplacementModel.old_image AS OldImage, replacement.ReplacementModel.new_image AS NewImage, 
        replacement.ReplacementModel.code AS Code, replacement.ReplacementModel.comment AS Comment,Channels.MeterNumber AS ChannelMeterNumber, 
        longitude, latitude, replacement.ReplacementModel.Billable,replacement.ReplacementModel.ReasonId, replacement.ReplacementModel.ChargeDescription, [user]
            FROM replacement.ReplacementModel
            LEFT JOIN replacement.NonAmrConsumers ON NonAmrConsumers.Id = ReplacementModel.NonAmrConsumerId
            LEFT JOIN Sites ON Sites.ID = ReplacementModel.site_id
            LEFT JOIN Channels ON Channels.ID = replacement.ReplacementModel.channel_id
            LEFT JOIN DiameterTypes ON DiameterTypes.ID = replacement.ReplacementModel.diameter
            WHERE ReplacementModel.handled = 0
        AND  replacement_type = 3";

        internal const string RowsToIgnoreQuery =
            @"SELECT	command_id AS CommandId, old_number AS OldNumber, new_number AS NewNumber,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, '', NonAmrConsumers.AddressLine1) AddressLine1,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, ReplacementModel.Address, NonAmrConsumers.AddressLine2) AddressLine2,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, ReplacementModel.account_name, NonAmrConsumers.FirstName) FirstName,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, '', NonAmrConsumers.LastName) LastName,
        ReplacementModel.site_id AS SiteID, Sites.SiteName, approved AS Approved,
        channel_id AS ChannelId, meter_model AS MeterModel, meter_manufacturer AS MeterManufacturer, meter_type AS MeterType,
        ReplacementModel.consumer_id AS Consumer,
        ReplacementModel.NonAmrConsumerId AS NonAmrConsumer,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, ISNULL(ReplacementModel.diameter, 3), NonAmrConsumers.Diameter) Diameter,
        replacement.ReplacementModel.old_image AS OldImage, replacement.ReplacementModel.new_image AS NewImage, 
        replacement.ReplacementModel.code AS Code, replacement.ReplacementModel.comment AS Comment,Channels.MeterNumber AS ChannelMeterNumber, 
        longitude, latitude, replacement.ReplacementModel.Billable,replacement.ReplacementModel.ReasonId, replacement.ReplacementModel.ChargeDescription, [user]
            FROM replacement.ReplacementModel
            LEFT JOIN replacement.NonAmrConsumers ON NonAmrConsumers.Id = ReplacementModel.NonAmrConsumerId
            LEFT JOIN Sites ON Sites.ID = ReplacementModel.site_id
            LEFT JOIN Channels ON Channels.ID = replacement.ReplacementModel.channel_id
            WHERE  replacement_type = 5";
        internal const string RowsToChargeQuery = 
            @"SELECT ReplacementModel.site_id AS SiteID, Sites.SiteName, Consumers.BilingID, 
                (AddressIndex.Name  + Property.AddressLine2) AS Address, 
                replacement.ReplacementModel.datetime AS [DateTime],
                longitude AS Longitude, 
                latitude AS Latitude,old_number AS OldNumber, new_number AS NewNumber, old_reading AS OldReading, 
                old_image AS OldImage, reading AS Reading, new_image AS NewImage,comment AS Comment,
                ReplacementModel.ChargeDescription,code AS Code, 
                code_description AS CodeDescription, Consumers.diameter AS Diameter, Consumers.ID AS ConsumerId, 
                ReasonId, ReplacementReason.Name AS ReplacementReason, ReplacementModel.Charged,ReplacementModel.command_id AS CommandId,
				replacement.ReplacementModel.Billable
            FROM replacement.ReplacementModel
	            LEFT JOIN Consumers ON Consumers.ID = ReplacementModel.consumer_id 
	            LEFT JOIN Sites ON Sites.ID = ReplacementModel.site_id
	            LEFT JOIN Property ON Property.ID = Consumers.Property
	            LEFT JOIN AddressIndex ON AddressIndex.ID = Property.AddressLine1
	            LEFT JOIN Payer ON Payer.ID = Consumers.PayerID
                LEFT JOIN Channels ON Channels.ID = replacement.ReplacementModel.channel_id
	            LEFT JOIN ReplacementReason ON ReplacementReason.Id = replacement.ReplacementModel.ReplacementReason
            WHERE (site_id = @SiteId OR @SiteId = 0)
	              AND  replacement.ReplacementModel.datetime BETWEEN @FromDate AND @ToDate
	              AND replacement.ReplacementModel.Billable = 1 
--OR ReplacementModel.Billable IS NULL
	              AND replacement.ReplacementModel.handled = 1";

        internal const string RowsToReplaceQuery = @"SELECT command_id AS	CommandId, old_number AS OldNumber, new_number AS NewNumber, NonAmrConsumers.AddressLine1,
        NonAmrConsumers.AddressLine2, NonAmrConsumers.FirstName, NonAmrConsumers.LastName,  ReplacementModel.site_id AS SiteID, Sites.SiteName, approved AS Approved,
        channel_id AS ChannelId, meter_model AS MeterModel, meter_manufacturer AS MeterManufacturer, meter_type AS MeterType,
        ReplacementModel.NonAmrConsumerId AS Consumer,
        ReplacementModel.NonAmrConsumerId AS NonAmrConsumer,
        DiameterTypes.Name AS Diameter,
        replacement.ReplacementModel.old_image AS OldImage, replacement.ReplacementModel.new_image AS NewImage, 
        replacement.ReplacementModel.code AS Code,replacement.ReplacementModel.comment AS Comment, Channels.MeterNumber AS ChannelMeterNumber, 
        longitude, latitude, replacement.ReplacementModel.Billable,replacement.ReplacementModel.ReasonId, replacement.ReplacementModel.ChargeDescription, [user]
          ,ReplacementModel.reading, ReplacementModel.old_reading AS OldReading
            FROM replacement.ReplacementModel
            JOIN replacement.NonAmrConsumers ON NonAmrConsumers.Id = ReplacementModel.NonAmrConsumerId
            LEFT JOIN Sites ON Sites.ID = ReplacementModel.site_id
            LEFT JOIN Channels ON Channels.ID = replacement.ReplacementModel.channel_id
            LEFT JOIN DiameterTypes ON DiameterTypes.ID = replacement.ReplacementModel.diameter
            WHERE ReplacementModel.handled = 0
        AND  replacement_type = 2";
        internal const string ReplacesQuery = @"WITH data AS (
     UPDATE     command
        SET        daily_exported = CURRENT_TIMESTAMP
        WHERE      command.date_time < CURRENT_DATE
                   AND city_code IS NULL
                   AND daily_exported IS NULL
        RETURNING  *
   )
   SELECT     command.id AS command_id,
              file,
              meter_id,
              COALESCE(command.address, command.notes) AS address,
              command.device_id AS ""user"",
              command.category AS ""action"",
              command.date_time AS ""datetime"",
              longitude, latitude, 
              old.meter_num AS old_number,
              old.reading AS old_reading,
              old.picture AS old_image,
              new.meter_num AS new_number,
              new.reading,
              new.picture AS new_image,
              comment_text AS comment,
              comment_code AS code,
              array_to_string(ARRAY[
                  CASE WHEN(comment_code & X'100000'::int) = X'100000'::int THEN 'המד נעלם (21)'  END,
                  CASE WHEN(comment_code & X'000001'::int) = X'000001'::int THEN 'כתובת לא נכונה (01)' END,
                  CASE WHEN(comment_code & X'000002'::int) = X'000002'::int THEN 'מקום סגור 9 (02)' END,
                  CASE WHEN(comment_code & X'000004'::int) = X'000004'::int THEN 'אין גישה (03)' END,
                  CASE WHEN(comment_code & X'000008'::int) = X'000008'::int THEN 'כלב (04)' END,
                  CASE WHEN(comment_code & X'001000'::int) = X'001000'::int THEN 'המונה מנותק מהצנרת (13)' END,
                  CASE WHEN(comment_code & X'010000'::int) = X'010000'::int THEN 'ניתוק מד גינתי (17)' END,
                  CASE WHEN(comment_code & X'002000'::int) = X'002000'::int THEN 'גניבת מים מרשת משותפת (14)' END,
                  CASE WHEN(comment_code & X'004000'::int) = X'004000'::int THEN 'מונה חדש (15)' END,
                  CASE WHEN(comment_code & X'000010'::int) = X'000010'::int THEN 'המונה מזיע' END,
                  CASE WHEN(comment_code & X'080000'::int) = X'080000'::int THEN 'מד פגום' END,
                  CASE WHEN(comment_code & X'000020'::int) = X'000020'::int THEN 'ברז לא סוגר' END,
                  CASE WHEN(comment_code & X'000040'::int) = X'000040'::int THEN 'אין ברז' END,
                  CASE WHEN(comment_code & X'000080'::int) = X'000080'::int THEN 'חלודה' END,
                  CASE WHEN(comment_code & X'008000'::int) = X'008000'::int THEN 'בוררות' END,
                  CASE WHEN(comment_code & X'000100'::int) = X'000100'::int THEN 'עצור (9)' END,
                  CASE WHEN(comment_code & X'000200'::int) = X'000200'::int THEN 'אין שימוש' END,
                  CASE WHEN(comment_code & X'000400'::int) = X'000400'::int THEN 'התמונה לא של המד' END,
                  CASE WHEN(comment_code & X'020000'::int) = X'020000'::int THEN 'המונה הפוך' END,
                  CASE WHEN(comment_code & X'040000'::int) = X'040000'::int THEN 'אין תקשורת' END,
                  CASE WHEN(comment_code & X'200000'::int) = X'200000'::int THEN 'ברז סגור' END,
                  CASE WHEN(comment_code & X'400000'::int) = X'400000'::int THEN 'התקנה לפי דרישה' END,
                  CASE WHEN(comment_code & X'800000'::int) = X'800000'::int THEN 'ניתוק לפי דרישה' END,
                  CASE WHEN(comment_code & X'0000100000000'::bigint) = X'0000100000000'::bigint THEN 'הוספת פשטיק' END,
                  CASE WHEN(comment_code & X'0000200000000'::bigint) = X'0000200000000'::bigint THEN 'הקטנת קוטר' END,
                  CASE WHEN(comment_code & X'0000400000000'::bigint) = X'0000400000000'::bigint THEN 'היעדר גישה – מד גבוה המצריך סולם' END,
                  CASE WHEN(comment_code & X'0001000000000'::bigint) = X'0001000000000'::bigint THEN 'הוספת הארקה' END,
                  CASE WHEN(comment_code & X'0004000000000'::bigint) = X'0004000000000'::bigint THEN 'צנרת ארוכה – דורש  עבודת קיצור ' END,
                  CASE WHEN(comment_code & X'0008000000000'::bigint) = X'0008000000000'::bigint THEN 'צנרת קצרה – דורש עבודת הארכה' END,
                  CASE WHEN(comment_code & X'0010000000000'::bigint) = X'0010000000000'::bigint THEN 'ברגים מרותכים לצנרת' END,
                  CASE WHEN(comment_code & X'0020000000000'::bigint) = X'0020000000000'::bigint THEN 'רקורד תקוע' END,
                  CASE WHEN(comment_code & X'0040000000000'::bigint) = X'0040000000000'::bigint THEN 'התקנת ברז לפני / אחרי המד' END,
                  CASE WHEN(comment_code & X'0080000000000'::bigint) = X'0080000000000'::bigint THEN 'כמות חורים לא מתאימה בפלאנץ''' END,
                  CASE WHEN(comment_code & X'0100000000000'::bigint) = X'0100000000000'::bigint THEN 'התקנת לוכד אבנים' END
              ], ' | ') AS code_description,
              CASE
                  WHEN (comment_code & X'01000000'::bigint) = X'01000000'::bigint THEN 2
                  WHEN (comment_code & X'02000000'::bigint) = X'02000000'::bigint THEN 3
                  WHEN (comment_code & X'04000000'::bigint) = X'04000000'::bigint THEN 4
                  WHEN (comment_code & X'08000000'::bigint) = X'08000000'::bigint THEN 5
                  WHEN (comment_code & X'10000000'::bigint) = X'10000000'::bigint THEN 6
                  WHEN (comment_code & X'20000000'::bigint) = X'20000000'::bigint THEN 7
                  WHEN (comment_code & X'40000000'::bigint) = X'40000000'::bigint THEN 8  
                  WHEN (comment_code & X'80000000'::bigint) = X'80000000'::bigint THEN 10 
              END AS diameter,
              account_name, owner_id, seq
    FROM data AS command
              LEFT JOIN entry AS old ON(old.command_id = command.id AND old.part = 0)
              LEFT JOIN entry AS new ON(new.command_id = command.id AND new.part = 1)
    ORDER BY  command.date_time";

        internal const string FindAndSetType = @"DECLARE @NewAssociations AS TABLE (
	[command_id] [bigint] NOT NULL,
	[meter_id] [bigint] NULL,
	[old_number] [bigint] NULL,
	[new_number] [bigint] NULL,
	[replacement_type] [int] NOT NULL DEFAULT 0,
	[consumer_id] [bigint] NULL,
	[NonAmrConsumerId] [bigint] NULL,
	[site_id] [int] NULL,
	[is_replacement] BIT NOT NULL,
	[meter_model] INT NOT NULL DEFAULT 1,
	[meter_manufacturer] INT NOT NULL DEFAULT 1,
	[meter_type] INT NOT NULL DEFAULT 1
);

INSERT	@NewAssociations([command_id], [meter_id], [old_number], [new_number], [consumer_id], [NonAmrConsumerId], [site_id], [is_replacement])
SELECT	[command_id], [meter_id], [old_number], [new_number], [consumer_id], [NonAmrConsumerId], [site_id], IIF(new_number = old_number, 0, 1)
FROM	replacement.ReplacementModel WITH (SERIALIZABLE)
WHERE	[replacement_type] = 0;

UPDATE	@NewAssociations
SET		[replacement_type] = 4
WHERE	[new_number] IN (SELECT MeterNumber FROM Consumers);

UPDATE	up
SET		[replacement_type] = IIF(data.Count = 1, 1, -1),
		[consumer_id] = data.ConsumerID,
		[site_id] = data.SiteID
FROM	@NewAssociations up
		JOIN (
			SELECT		MeterNumber,
						COUNT(*) AS Count,
						IIF(COUNT(*) = 1, MAX(ID), NULL) AS ConsumerID,
						IIF(COUNT(*) = 1, MAX(SiteID), NULL) AS SiteID
			FROM		Consumers
			GROUP BY	MeterNumber
		) AS data ON data.MeterNumber = up.old_number
WHERE	up.[replacement_type] = 0 AND up.[is_replacement] = 1 AND up.site_id IS NULL; 

UPDATE	up
SET		[replacement_type] = IIF(data.Count = 1, 1, -1),
		[consumer_id] = data.ConsumerID
FROM	@NewAssociations up
		JOIN (
			SELECT		MeterNumber, SiteID,
						COUNT(*) AS Count,
						IIF(COUNT(*) = 1, MAX(ID), NULL) AS ConsumerID
			FROM		Consumers
			GROUP BY	MeterNumber, SiteID
		) AS data ON data.MeterNumber = up.old_number AND data.SiteID = up.site_id
WHERE	up.[replacement_type] = 0 AND up.[is_replacement] = 1 AND up.site_id IS NOT NULL;

UPDATE	up
SET		[replacement_type] = IIF(NonAmrConsumers.MeterNumber IS NULL, 3, 2),
		[NonAmrConsumerId] = NonAmrConsumers.ID
FROM	@NewAssociations up
		JOIN replacement.NonAmrConsumers ON NonAmrConsumers.BilingID = up.meter_id AND NonAmrConsumers.SiteID = up.site_id
WHERE	up.[replacement_type] = 0 AND up.[is_replacement] = 1 AND up.site_id IS NOT NULL;

UPDATE	up
SET		[replacement_type] = IIF(data.Invalid = 0, 2, -1),
		[NonAmrConsumerId] = data.ConsumerID,
		[site_id] = data.SiteID
FROM	@NewAssociations up
		JOIN (
			SELECT		MeterNumber,
						IIF(COUNT(*) = 1, MAX(Handled + 1), 1) AS Invalid,
						IIF(COUNT(*) = 1, MAX(ID), NULL) AS ConsumerID,
						IIF(COUNT(*) = 1, MAX(SiteID), NULL) AS SiteID
			FROM		replacement.NonAmrConsumers
			GROUP BY	MeterNumber
		) AS data ON data.MeterNumber = up.old_number
WHERE	up.[replacement_type] = 0 AND up.[is_replacement] = 1 AND up.site_id IS NULL; 

UPDATE	up
SET		[replacement_type] = IIF(data.Invalid = 0, 2, -1),
		[NonAmrConsumerId] = data.ConsumerID
FROM	@NewAssociations up
		JOIN (
			SELECT		MeterNumber, SiteID,
						IIF(COUNT(*) = 1, MAX(Handled + 1), 1) AS Invalid,
						IIF(COUNT(*) = 1, MAX(ID), NULL) AS ConsumerID
			FROM		replacement.NonAmrConsumers
			GROUP BY	MeterNumber, SiteID
		) AS data ON data.MeterNumber = up.old_number AND data.SiteID = up.site_id
WHERE	up.[replacement_type] = 0 AND up.[is_replacement] = 1 AND up.site_id IS NOT NULL;

UPDATE	up
SET		replacement_type = data.replacement_type,
		consumer_id = data.consumer_id,
		NonAmrConsumerId = data.NonAmrConsumerId,
		site_id = data.site_id,
		meter_type = data.meter_type,
		meter_manufacturer = data.meter_manufacturer,
		meter_model = data.meter_model
FROM	replacement.ReplacementModel AS up
		JOIN @NewAssociations data ON data.command_id = up.command_id
WHERE	data.replacement_type > 0;";
        internal const string SetChannelNotAssociated = @"UPDATE	up
                                                         SET		channel_id = Channels.ID
                                                         FROM	replacement.ReplacementModel AS up
		                                                            CROSS APPLY (
			                                                            SELECT		MAX(ID) AS ID
			                                                            FROM		Channels
			                                                            WHERE		MeterNumber = up.new_number
			                                                            HAVING		COUNT(*) = 1
		                                                            ) AS Channels
		                                                            LEFT JOIN Consumers ON Consumers.Channel = Channels.ID
                                                         WHERE	up.replacement_type > 0
		                                                            AND channel_id IS NULL
		                                                            AND Consumers.ID IS NULL";
        internal const string GetChannelsQuery = @"SELECT	DISTINCT Channels.ID AS ChannelID, EndUnits.EndUnit, Channels.ChannelIndex, EndUnits.LastReceived, EndUnits.Site,
		      IIF(Consumers.ID IS NULL, 0, 1) AS HasConsumer
              FROM	Channels
		        JOIN EndUnits ON EndUnits.ID = Channels.EndUnit
		        LEFT JOIN Consumers ON Consumers.Channel = Channels.ID
              WHERE	Channels.MeterNumber = @MeterNumber";
        internal const string CreateConsumersQuery = @"DECLARE @approved Table(
	[command_id] [bigint] NOT NULL,
	[file] [varchar](255) NULL,
	[meter_id] [bigint] NULL,
	[address] [varchar](255) NULL,
	[user] [int] NULL,
	[action] [smallint] NULL,
	[datetime] [datetime] NULL,
	[longitude] [float] NULL,
	[latitude] [float] NULL,
	[old_number] [bigint] NULL,
	[old_reading] [int] NULL,
	[old_image] [nvarchar](MAX) NULL,
	[new_number] [bigint] NULL,
	[reading] [int] NULL,
	[new_image] [nvarchar](MAX) NULL,
	[comment] [nvarchar](MAX) NULL,
	[code] [bigint] NULL,
	[code_description] [nvarchar](MAX) NULL,
	[diameter] [int] NULL,
	[replacement_type] [int] NULL,
	[consumer_id] [bigint] NULL,
	[NonAmrConsumerId] [bigint] NULL,
	[approved] [bit] NOT NULL,
	[handled] [bit] NULL,
	[channel_id] [bigint] NULL,
	[meter_model] [int] NULL,
	[meter_manufacturer] [int] NULL,
	[meter_type] [int] NULL,
    [account_name] [nvarchar](50) NULL, 
    [owner_id] [bigint] NULL, 
    [seq][bigint] NULL, 

	[SiteID] [bigint] NOT NULL,
	[BilingID] [bigint] NOT NULL,
	[MeterNumber] [bigint] NULL,
	[MaterialUse] [int] NULL,
	[LocationRemark] [nvarchar](50)  NULL,
    [ConsumerDiameter] [int]  NULL,
	[GroupSigment] [int]  NULL,
	[Group1] [int]  NULL,
	[Group2] [int]  NULL,
	[Group3] [int]  NULL,
	[Sigment1] [int]  NULL,
	[Sigment2] [int]  NULL,
	[SortNumber] [int] NULL,
	[WalkOrder] [nvarchar](255) NULL,
	[MeterService] [int]  NULL,
	[Special] [bit]  NULL,
	[gridJob] [int]  NULL,
	[consumeType] [int]  NULL,
	[GisX] [float] NULL,
	[GisY] [float] NULL,
	[GisZ] [float] NULL,
	[DeviceType] [int] NULL,
	[IconType] [int] NULL,
	[PayerID] [nvarchar](50) NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[PayerAddressLine1] [nvarchar](50) NULL,
	[PayerAddressLine2] [nvarchar](50) NULL,
	[PayerCity] [nvarchar](50) NULL,
	[PayerZipCode] [nvarchar](10) NULL,
	[Telephone1] [nvarchar](50) NULL,
	[Telephone2] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[PayerTypeID] [int]  NULL,
	[PropertyID] [bigint]  NULL,
	[AddressLine1] [nvarchar](255)  NULL,
	[AddressLine2] [nvarchar](50)  NULL,
	[City] [nvarchar](50)  NULL,
	[ZipCode] [int]  NULL,
	[ProprtyTypeID] [int]  NULL
);

INSERT INTO @approved([command_id]
           ,[file]
           ,[meter_id]
           ,[address]
           ,[user]
           ,[action]
           ,[datetime]
           ,[longitude]
           ,[latitude]
           ,[old_number]
           ,[old_reading]
           ,[old_image]
           ,[new_number]
           ,[reading]
           ,[new_image]
           ,[comment]
           ,[code]
           ,[code_description]
           ,[diameter]
           ,[replacement_type]
           ,[consumer_id]
           ,[NonAmrConsumerId]
           ,[approved]
           ,[handled]
           ,[channel_id]
           ,[meter_model]
           ,[meter_manufacturer]
           ,[meter_type]
           ,[account_name]
           ,[owner_id]
           ,[seq]

		  ,[SiteID]
		  ,[BilingID]
		  ,[MeterNumber]
		  ,[MaterialUse]
		  ,[LocationRemark]
          ,[ConsumerDiameter]
		  ,[GroupSigment]
		  ,[Group1]
		  ,[Group2]
		  ,[Group3]
		  ,[Sigment1]
		  ,[Sigment2]
		  ,[SortNumber]
		  ,[WalkOrder]
		  ,[MeterService]
		  ,[Special]
		  ,[gridJob]
		  ,[consumeType]
		  ,[GisX]
		  ,[GisY]
		  ,[GisZ]
		  ,[DeviceType]
		  ,[IconType]
		  ,[PayerID]
		  ,[FirstName]
		  ,[LastName]
		  ,[PayerAddressLine1]
		  ,[PayerAddressLine2]
		  ,[PayerCity]
		  ,[PayerZipCode]
		  ,[Telephone1]
		  ,[Telephone2]
		  ,[Email]
		  ,[PayerTypeID]
		  ,[PropertyID]
		  ,[AddressLine1]
		  ,[AddressLine2]
		  ,[City]
		  ,[ZipCode]
		  ,[ProprtyTypeID])
SELECT [ReplacementModel].[command_id]
           ,[ReplacementModel].[file]
           ,[ReplacementModel].[meter_id]
           ,[ReplacementModel].[address]
           ,[ReplacementModel].[user]
           ,[ReplacementModel].[action]
           ,[ReplacementModel].[datetime]
           ,[ReplacementModel].[longitude]
           ,[ReplacementModel].[latitude]
           ,[ReplacementModel].[old_number]
           ,[ReplacementModel].[old_reading]
           ,[ReplacementModel].[old_image]
           ,[ReplacementModel].[new_number]
           ,[ReplacementModel].[reading]
           ,[ReplacementModel].[new_image]
           ,[ReplacementModel].[comment]
           ,[ReplacementModel].[code]
           ,[ReplacementModel].[code_description]
           ,[ReplacementModel].[diameter]
           ,[ReplacementModel].[replacement_type]
           ,[ReplacementModel].[consumer_id]
           ,[ReplacementModel].[NonAmrConsumerId]
           ,[ReplacementModel].[approved]
           ,[ReplacementModel].[handled]
           ,ISNULL([ReplacementModel].[channel_id], 543)
           ,[ReplacementModel].[meter_model]
           ,[ReplacementModel].[meter_manufacturer]
           ,[ReplacementModel].[meter_type]
           ,[ReplacementModel].[account_name]
           ,[ReplacementModel].[owner_id]
           ,[ReplacementModel].[seq]


      ,ISNULL([NonAmrConsumers].[SiteID], [ReplacementModel].site_id)
      ,COALESCE([NonAmrConsumers].[BilingID], [meter_id], -[new_number])
      ,[NonAmrConsumers].[MeterNumber]
      ,[NonAmrConsumers].[MaterialUse]
      ,[NonAmrConsumers].[LocationRemark]
      ,[NonAmrConsumers].[Diameter]
      ,[NonAmrConsumers].[GroupSigment]
      ,[NonAmrConsumers].[Group1]
      ,[NonAmrConsumers].[Group2]
      ,[NonAmrConsumers].[Group3]
      ,[NonAmrConsumers].[Sigment1]
      ,[NonAmrConsumers].[Sigment2]
      ,[NonAmrConsumers].[SortNumber]
      ,COALESCE([NonAmrConsumers].[WalkOrder], [ReplacementModel].[seq]) AS [WalkOrder]
      ,[NonAmrConsumers].[MeterService]
      ,[NonAmrConsumers].[Special]
      ,[NonAmrConsumers].[gridJob]
      ,[NonAmrConsumers].[consumeType]
      ,[NonAmrConsumers].[GisX]
      ,[NonAmrConsumers].[GisY]
      ,[NonAmrConsumers].[GisZ]
      ,[NonAmrConsumers].[DeviceType]
      ,[NonAmrConsumers].[IconType]
      ,COALESCE([NonAmrConsumers].[PayerID], [ReplacementModel].[owner_id]) AS [PayerID]
      ,COALESCE([NonAmrConsumers].[FirstName], [ReplacementModel].[account_name]) AS [FirstName]
      ,[NonAmrConsumers].[LastName]
      ,[NonAmrConsumers].[PayerAddressLine1]
      ,[NonAmrConsumers].[PayerAddressLine2]
      ,[NonAmrConsumers].[PayerCity]
      ,[NonAmrConsumers].[PayerZipCode]
      ,[NonAmrConsumers].[Telephone1]
      ,[NonAmrConsumers].[Telephone2]
      ,[NonAmrConsumers].[Email]
      ,[NonAmrConsumers].[PayerTypeID]
      ,[NonAmrConsumers].[PropertyID]
      ,[NonAmrConsumers].[AddressLine1]
      ,[NonAmrConsumers].[AddressLine2]
      ,[NonAmrConsumers].[City]
      ,[NonAmrConsumers].[ZipCode]
      ,[NonAmrConsumers].[ProprtyTypeID]
FROM [replacement].[ReplacementModel]
		LEFT JOIN [replacement].[NonAmrConsumers] ON [NonAmrConsumers].ID = [ReplacementModel].[NonAmrConsumerId]
WHERE  [ReplacementModel].handled = 0 AND approved = 1 AND replacement_type = @Type AND (site_id = @SiteId OR @SiteId = 0);

DECLARE @PayerIds TABLE (
	[command_id] [bigint] NOT NULL,
	[payer_id] [bigint] NOT NULL
)

MERGE	INTO [dbo].[Payer] AS target
USING	@approved AS source
ON		1 = 2
WHEN    NOT MATCHED BY TARGET THEN
		INSERT ([PayerID], [FirstName], [LastName], [AddressLine1], [AddressLine2], [City], [ZipCode], [Telephone1], [Telephone2], [Email], [PayerTypeID], [Site])
		VALUES (ISNULL(source.[PayerID], -ABS(source.[BilingID])), ISNULL(source.[FirstName], ''),
                ISNULL(source.[LastName], ''),ISNULL(source.[PayerAddressLine1],''),
                ISNULL(source.[PayerAddressLine2],''),ISNULL(source.[PayerCity],''), 
                ISNULL(source.[PayerZipCode],''),
		        source.[Telephone1], source.[Telephone2], source.[Email], ISNULL(source.[PayerTypeID],1),
                source.[SiteID])
OUTPUT	source.command_id, inserted.ID INTO @PayerIds([command_id], [payer_id]);

DECLARE @AddressIndexes TABLE (
	[command_id] [bigint] NOT NULL,
	[AddressIndex_id] [bigint] NOT NULL
)

MERGE	INTO [dbo].[AddressIndex] AS target
USING	@approved AS source
ON		1 = 2
WHEN    NOT MATCHED BY TARGET THEN
		INSERT ([Name])
		VALUES (ISNULL(source.[AddressLine1],''))
OUTPUT	source.command_id, inserted.ID INTO @AddressIndexes([command_id], [AddressIndex_id]);

DECLARE @PropertyIds TABLE (
	[command_id] [bigint] NOT NULL,
	[Property_id] [bigint] NOT NULL
)

MERGE	INTO [dbo].[Property] AS target
USING	(
			SELECT	a.command_id, ISNULL(a.[PropertyID], -ABS(a.[BilingID])) AS [PropertyID], p.[payer_id] AS [Payer],
                    i.[AddressIndex_id] AS [AddressLine1], ISNULL(a.[AddressLine2], a.[address]) AS [AddressLine2],
                    ISNULL(a.[City], '')AS [City],ISNULL(a.[ZipCode],'') AS [ZipCode] ,ISNULL(a.[ProprtyTypeID],1) AS [ProprtyTypeID], a.[SiteId]
			FROM	@approved AS a
					JOIN @PayerIds as p ON p.command_id = a.command_id
					JOIN @AddressIndexes as i ON i.command_id = a.command_id 
		)
		AS source
ON		1 = 2
WHEN    NOT MATCHED BY TARGET THEN
		INSERT ([PropertyID], [Payer], [AddressLine1], [AddressLine2], [City], [ZipCode], [ProprtyTypeID], [Size], [Site])
		VALUES (source.[PropertyID], source.[Payer], source.[AddressLine1], source.[AddressLine2], source.[City], source.[ZipCode],
		        ISNULL(source.[ProprtyTypeID], 1), 0, source.[SiteId])
OUTPUT	source.command_id, inserted.ID INTO @PropertyIds([command_id], [Property_id]);

DECLARE @ConsumerIds TABLE (
	[command_id] [bigint] NOT NULL,
	[consumer_id] [bigint] NOT NULL
)
MERGE	INTO [dbo].[Consumers] AS target
USING	(
			SELECT a.[SiteID], a.[channel_id], a.[BilingID], pr.[Property_id], pa.[payer_ID], a.[new_number],ISNULL(a.[MaterialUse], 0) AS MaterialUse, ISNULL(a.[LocationRemark],'') AS [LocationRemark],
					STUFF(CONCAT(
						IIF(a.old_image LIKE '/%', '&~O=' + SUBSTRING(a.old_image, 2, LEN(a.old_image) - 1), ''),
						IIF(a.new_image LIKE '/%', '&~N=' + SUBSTRING(a.new_image, 2, LEN(a.new_image) - 1), '')
					), 1, 1, '') AS [PictureLink],
					COALESCE(a.[diameter], a.ConsumerDiameter, 3) AS [Diameter], a.datetime AS [Established],
					ISNULL(a.[GroupSigment],0) AS [GroupSigment],ISNULL(a.[Group1],0) AS [Group1],ISNULL(a.[Group2],0) AS [Group2],ISNULL(a.[Group3],0) AS [Group3],ISNULL(a.[Sigment1],0) AS [Sigment1],ISNULL(a.[Sigment2],0) AS [Sigment2], ISNULL(a.[SortNumber], 0) AS [SortNumber], 
                    ISNULL(a.[WalkOrder], '') AS [WalkOrder], ISNULL(a.[MeterService],2) AS [MeterService], ISNULL(a.[Special],1) AS [Special],     
                    a.meter_manufacturer, a.meter_model, a.meter_type,
					ISNULL(a.[gridJob],4) AS [gridJob],ISNULL(a.[consumeType],1) AS [consumeType], ISNULL(a.longitude,a.[GisX]) AS [GisX],ISNULL(a.latitude, a.[GisY]) AS [GisY], 
                    ISNULL(a.[GisZ],IIF(a.[GisX] IS NULL OR a.[GisY] IS NULL, NULL, 1)) AS [GisZ], ISNULL(a.[DeviceType],1) AS [DeviceType],ISNULL(a.[IconType],1) AS [IconType], a.command_id
			FROM	@approved AS a
					JOIN @PayerIds as pa ON pa.command_id = a.command_id
					JOIN @PropertyIds as pr ON pr.command_id = a.command_id
		)
		AS source
ON		1 = 2
WHEN    NOT MATCHED BY TARGET THEN
		INSERT ([SiteID], [Channel], [BilingID], [Property], [PayerID], [MeterNumber], [MaterialUse], [LocationRemark], [PictureLink], [Diameter], [Established], 
                [GroupSigment], [Group1], [Group2], [Group3], [Sigment1], [Sigment2], [SortNumber], [WalkOrder], [MeterService], [Special], [meterManufacturer], [meterModel], [meterType], [gridJob], [consumeType], [GisX], [GisY], [GisZ], [DeviceType], [IconType])
		VALUES (source.[SiteID], source.[channel_id], source.[BilingID], source.[Property_id], source.[payer_ID], source.[new_number], source.MaterialUse, source.[LocationRemark],
                source.[PictureLink], source.[Diameter], source.[Established], source.[GroupSigment], source.[Group1], source.[Group2], source.[Group3],source.[Sigment1],source.[Sigment2],source.[SortNumber],
                source.[WalkOrder], source.[MeterService], source.[Special], source.meter_manufacturer, source.meter_model, source.meter_type, 
                source.[gridJob], source.[consumeType], source.[GisX],source.[GisY],source.[GisZ],source.[DeviceType], source.[IconType])
OUTPUT	source.command_id, inserted.ID INTO @ConsumerIds([command_id], [consumer_id]);
UPDATE	up
SET		handled = IIF([channel_id] IS NULL, 0, 1),
		approved = IIF([channel_id] IS NULL, 0, approved),
		replacement_type = IIF([channel_id] IS NULL, IIF(@Type=2,7,8), replacement_type),
        consumer_id = source.consumer_id
FROM	[replacement].[ReplacementModel] AS up
		JOIN @ConsumerIds source ON up.command_id = source.command_id;

UPDATE  replacement.NonAmrConsumers
SET		Handled = 1
WHERE	ID IN (SELECT NonAmrConsumerId FROM @approved);

SELECT   (Payer.FirstName + ' ' + Payer.LastName) AS FullName,  meter_id AS MeterId,	address AS Address,	datetime AS DateTime,	longitude AS Longitude,	latitude AS Latitude,	old_number AS OldNumber,
        old_reading AS OldReading,	IIF(LEN(old_image) > 0,'=HYPERLINK(""http://reader.madrimonim.co.il/reading_images' +  old_image +  '"")',NULL) AS OldImage,	
        new_number AS NewNumber,	reading AS Reading,	IIF(LEN(old_image) > 0,'=HYPERLINK(""http://reader.madrimonim.co.il/reading_images' + new_image + '"")', NULL)   AS NewImage,
        DiameterTypes.inch AS Diameter, comment AS Comment
FROM    @approved AS approved
        LEFT JOIN Consumers on Consumers.ID = approved.consumer_id
		LEFT JOIN Payer on Payer.ID = Consumers.PayerID
        LEFT JOIN DiameterTypes ON DiameterTypes.ID = approved.diameter;";
        internal const string PerformedManuallyQuery = @"UPDATE replacement.ReplacementModel
                                                        SET handled=1,
	                                                        [manual] =1
                                                        WHERE command_id=@CommandId AND replacement_type=0";

        internal const string RemoveFromBillingQuery = @"UPDATE replacement.ReplacementModel
                                                        SET Billable = 0
                                                        WHERE command_id = @CommandId ";
        internal const string IgnoreMeterQuery = @"UPDATE replacement.ReplacementModel
                                                    SET replacement_type = 5,
	                                                    [manual] = 1
                                                    WHERE command_id = @CommandId ";
        internal const string DeleteSitesConsumersQuery = @"DELETE
                                                            FROM replacement.NonAmrConsumers
                                                            WHERE SiteID=@SiteID";
        internal const string GetConsumerQuery = @"SELECT	Consumers.ID, Consumers.SiteID, Consumers.BilingID, Consumers.MeterNumber, Payer.FirstName, Payer.LastName, AddressIndex.Name AS AddressLine1, Property.AddressLine2, 1 AS replacement_type
                        FROM	Consumers
		                        JOIN Payer ON Payer.ID = Consumers.PayerID
		                        JOIN Property ON Property.ID = Consumers.Property
		                        JOIN AddressIndex ON AddressIndex.ID = Property.AddressLine1
                        WHERE	SiteID=@SiteID
		                        AND (NULLIF(@MeterNumber, '') IS NULL OR Consumers.MeterNumber = @MeterNumber)
		                        AND (NULLIF(@BilingID, '') IS NULL OR Consumers.BilingID = @BilingID)
		                        AND (NULLIF(@PropertyID, '') IS NULL OR Property.PropertyID = @PropertyID)
                        UNION
                        SELECT	ID,SiteID, BilingID, MeterNumber, FirstName, LastName, AddressLine1, AddressLine2, IIF(MeterNumber IS NULL, 3, 2) AS replacement_type
                        FROM	replacement.NonAmrConsumers 
                        WHERE	Handled = 0
		                        AND SiteID=@SiteID
		                        AND (NULLIF(@MeterNumber, '') IS NULL OR MeterNumber = @MeterNumber)
		                        AND (NULLIF(@BilingID, '') IS NULL OR BilingID = @BilingID)
		                        AND (NULLIF(@PropertyID, '') IS NULL OR PropertyID = @PropertyID)";

        internal const string ChangeTypeQuery = @"UPDATE	replacement.ReplacementModel
                                                 SET		replacement_type = @NewType,
		                                                    consumer_id = IIF(@NewType = 1, @ConsumerId, consumer_id),
		                                                    NonAmrConsumerId = IIF(@NewType IN (2,3), @ConsumerId, NonAmrConsumerId),
                                                            site_id = @SiteId,
                                                            manual = 1
                                                 WHERE	    replacement_type = 0
		                                                    AND command_id = @CommandId";

        internal const string SetAsNewInstallation = @"UPDATE	replacement.ReplacementModel
                                                 SET		replacement_type = 3,
                                                            site_id = @SiteId,
                                                            manual = 1
                                                 WHERE	    replacement_type = 0
		                                                    AND command_id IN @CommandIds";
        internal const string GetSitesQuery = @"SELECT ID, SiteName
                                                FROM Sites
                                                WHERE ID IN (
				                                                SELECT site FROM UserSites WHERE UserID=1
			                                                )";
        internal const string RemoveRowsToSiteQuery = @" UPDATE replacement.ReplacementModel
                                                         SET site_id = @SiteId
                                                         WHERE command_id in @CommandIds AND consumer_id IS NULL AND NonAmrConsumerId IS NULL";

        internal const string GetReasonsQuery = @"SELECT Id, Name
                                                  FROM BillingReason";
        internal const string  RestoreMeterAsyncQuery = @"DECLARE @NewAssociations AS TABLE (
	                [command_id] [bigint] NOT NULL,
	                [meter_id] [bigint] NULL,
	                [old_number] [bigint] NULL,
	                [new_number] [bigint] NULL,
	                [replacement_type] [int] NOT NULL DEFAULT 0,
	                [consumer_id] [bigint] NULL,
	                [NonAmrConsumerId] [bigint] NULL,
	                [site_id] [int] NULL,
	                [is_replacement] BIT NOT NULL,
	                [meter_model] INT NOT NULL DEFAULT 1,
	                [meter_manufacturer] INT NOT NULL DEFAULT 1,
	                [meter_type] INT NOT NULL DEFAULT 1
                );

                UPDATE	replacement.ReplacementModel WITH (SERIALIZABLE)
                SET		[replacement_type] = 0
                OUTPUT  inserted.[command_id], inserted.[meter_id], inserted.[old_number], inserted.[new_number], inserted.[consumer_id],
		                inserted.[NonAmrConsumerId], inserted.[site_id], IIF(inserted.new_number = inserted.old_number, 0, 1)
                INTO	@NewAssociations([command_id], [meter_id], [old_number], [new_number], [consumer_id], [NonAmrConsumerId], [site_id], [is_replacement])
                WHERE	[command_id] = @CommandId;

                UPDATE	@NewAssociations
                SET		[replacement_type] = 4
                WHERE	[new_number] IN (SELECT MeterNumber FROM Consumers);

                UPDATE	up
                SET		[replacement_type] = IIF(data.Count = 1, 1, -1),
		                [consumer_id] = data.ConsumerID,
		                [site_id] = data.SiteID
                FROM	@NewAssociations up
		                JOIN (
			                SELECT		MeterNumber,
						                COUNT(*) AS Count,
						                IIF(COUNT(*) = 1, MAX(ID), NULL) AS ConsumerID,
						                IIF(COUNT(*) = 1, MAX(SiteID), NULL) AS SiteID
			                FROM		Consumers
			                GROUP BY	MeterNumber
		                ) AS data ON data.MeterNumber = up.old_number
                WHERE	up.[replacement_type] = 0 AND up.[is_replacement] = 1 AND up.site_id IS NULL; 

                UPDATE	up
                SET		[replacement_type] = IIF(data.Count = 1, 1, -1),
		                [consumer_id] = data.ConsumerID
                FROM	@NewAssociations up
		                JOIN (
			                SELECT		MeterNumber, SiteID,
						                COUNT(*) AS Count,
						                IIF(COUNT(*) = 1, MAX(ID), NULL) AS ConsumerID
			                FROM		Consumers
			                GROUP BY	MeterNumber, SiteID
		                ) AS data ON data.MeterNumber = up.old_number AND data.SiteID = up.site_id
                WHERE	up.[replacement_type] = 0 AND up.[is_replacement] = 1 AND up.site_id IS NOT NULL;

                UPDATE	up
                SET		[replacement_type] = IIF(NonAmrConsumers.MeterNumber IS NULL, 3, 2),
		                [NonAmrConsumerId] = NonAmrConsumers.ID
                FROM	@NewAssociations up
		                JOIN replacement.NonAmrConsumers ON NonAmrConsumers.BilingID = up.meter_id AND NonAmrConsumers.SiteID = up.site_id
                WHERE	up.[replacement_type] = 0 AND up.[is_replacement] = 1 AND up.site_id IS NOT NULL;

                UPDATE	up
                SET		[replacement_type] = IIF(data.Invalid = 0, 2, -1),
		                [NonAmrConsumerId] = data.ConsumerID,
		                [site_id] = data.SiteID
                FROM	@NewAssociations up
		                JOIN (
			                SELECT		MeterNumber,
						                IIF(COUNT(*) = 1, MAX(Handled + 1), 1) AS Invalid,
						                IIF(COUNT(*) = 1, MAX(ID), NULL) AS ConsumerID,
						                IIF(COUNT(*) = 1, MAX(SiteID), NULL) AS SiteID
			                FROM		replacement.NonAmrConsumers
			                GROUP BY	MeterNumber
		                ) AS data ON data.MeterNumber = up.old_number
                WHERE	up.[replacement_type] = 0 AND up.[is_replacement] = 1 AND up.site_id IS NULL; 

                UPDATE	up
                SET		[replacement_type] = IIF(data.Invalid = 0, 2, -1),
		                [NonAmrConsumerId] = data.ConsumerID
                FROM	@NewAssociations up
		                JOIN (
			                SELECT		MeterNumber, SiteID,
						                IIF(COUNT(*) = 1, MAX(Handled + 1), 1) AS Invalid,
						                IIF(COUNT(*) = 1, MAX(ID), NULL) AS ConsumerID
			                FROM		replacement.NonAmrConsumers
			                GROUP BY	MeterNumber, SiteID
		                ) AS data ON data.MeterNumber = up.old_number AND data.SiteID = up.site_id
                WHERE	up.[replacement_type] = 0 AND up.[is_replacement] = 1 AND up.site_id IS NOT NULL;

                UPDATE	up
                SET		replacement_type = data.replacement_type,
		                consumer_id = data.consumer_id,
		                NonAmrConsumerId = data.NonAmrConsumerId,
		                site_id = data.site_id,
		                meter_type = data.meter_type,
		                meter_manufacturer = data.meter_manufacturer,
		                meter_model = data.meter_model
                FROM	replacement.ReplacementModel AS up
		                JOIN @NewAssociations data ON data.command_id = up.command_id
                WHERE	data.replacement_type > 0";

        internal const string GetHandledRows =
            @"SELECT	command_id AS CommandId, old_number AS OldNumber, new_number AS NewNumber,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, '', NonAmrConsumers.AddressLine1) AddressLine1,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, ReplacementModel.Address, NonAmrConsumers.AddressLine2) AddressLine2,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, ReplacementModel.account_name, NonAmrConsumers.FirstName) FirstName,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, '', NonAmrConsumers.LastName) LastName,
        ReplacementModel.site_id AS SiteID, Sites.SiteName, approved AS Approved,
        channel_id AS ChannelId, meter_model AS MeterModel, meter_manufacturer AS MeterManufacturer, meter_type AS MeterType,
        ReplacementModel.consumer_id AS Consumer,
        ReplacementModel.NonAmrConsumerId AS NonAmrConsumer,
		IIF(ReplacementModel.NonAmrConsumerId IS NULL, ISNULL(ReplacementModel.diameter, 3), NonAmrConsumers.Diameter) Diameter,
        replacement.ReplacementModel.old_image AS OldImage, replacement.ReplacementModel.new_image AS NewImage, 
        replacement.ReplacementModel.code AS Code, replacement.ReplacementModel.comment AS Comment,Channels.MeterNumber AS ChannelMeterNumber, 
        longitude, latitude, replacement.ReplacementModel.Billable,replacement.ReplacementModel.ReasonId, 
        replacement.ReplacementModel.ChargeDescription, [user],replacement.ReplacementModel.datetime AS ReplaceDate
            FROM replacement.ReplacementModel
            LEFT JOIN replacement.NonAmrConsumers ON NonAmrConsumers.Id = ReplacementModel.NonAmrConsumerId
            JOIN Sites ON Sites.ID = ReplacementModel.site_id
            LEFT JOIN Channels ON Channels.ID = replacement.ReplacementModel.channel_id
            WHERE ReplacementModel.handled = 1
            AND  replacement.ReplacementModel.datetime BETWEEN @FromDate AND @ToDate";

        internal const string GetElementsQuery = @"SELECT [ID], [Name] 
                                              FROM replacement.Elements";
        internal const string GetConsumerElementsQuery = @"
                                SELECT replacement.Elements.Id, replacement.Elements.Name, replacement.ReplacementModelElements.Quantity, 
		                                IIF(ReplacementModelElements.command_id IS NULL, CAST(0 AS BIT),  CAST(1 AS BIT)) AS IsAssociated
                                FROM replacement.Elements
                                LEFT JOIN replacement.ReplacementModelElements 
	                                ON replacement.Elements.Id = replacement.ReplacementModelElements.ElementId 
	                                AND replacement.ReplacementModelElements.command_id=@CommandId";
      
        internal const string UpdateConsumerElementsQuery = @"
                            WITH target AS (
	                            SELECT * FROM [replacement].[ReplacementModelElements] WHERE command_id = @CommandId
                            )
                            MERGE	INTO target WITH (SERIALIZABLE) AS target 
                            USING	@Data AS source
                            ON		source.ElementId = target.ElementId
                            WHEN MATCHED THEN
	                            UPDATE SET Quantity = source.Quantity
                            WHEN NOT MATCHED BY TARGET THEN
	                            INSERT ([ElementId], command_id, [Quantity])
	                            VALUES (source.[ElementId], @CommandId, source.[Quantity])
                            WHEN NOT MATCHED BY SOURCE THEN
	                            DELETE;";
        internal const string AddNewElementQuery= @"INSERT replacement.Elements (Name)  
                                                    SELECT @Name
                                                    WHERE NOT EXISTS 
	                                                    (
		                                                    SELECT Name 
		                                                    FROM replacement.Elements  
		                                                    WHERE Name = @Name
	                                                    );";

        internal const string UpdateDuplicatesAsyncQuery = @"UPDATE	up 
                                                             SET		replacement_type = 
                                                                        IIF(d.NewNumber IS NOT NULL, 0, up.replacement_type),
		                                                            old_number = ISNULL(d.OldNumber, up.old_number),
		                                                            new_number = ISNULL(d.NewNumber, up.new_number)
                                                             FROM	replacement.ReplacementModel AS up
		                                                            JOIN @Data AS d ON d.CommandId = up.command_id;";

        internal const string SaveMissingChannelsQuery = @"DECLARE @approved Table(
	[command_id] [bigint] NOT NULL,
	[file] [varchar](255) NULL,
	[meter_id] [bigint] NULL,
	[address] [varchar](255) NULL,
	[user] [int] NULL,
	[action] [smallint] NULL,
	[datetime] [datetime] NULL,
	[longitude] [float] NULL,
	[latitude] [float] NULL,
	[old_number] [bigint] NULL,
	[old_reading] [int] NULL,
	[old_image] [nvarchar](MAX) NULL,
	[new_number] [bigint] NULL,
	[reading] [int] NULL,
	[new_image] [nvarchar](MAX) NULL,
	[comment] [text] NULL,
	[code] [bigint] NULL,
	[code_description] [text] NULL,
	[diameter] [int] NULL,
	[replacement_type] [int] NULL,
	[consumer_id] [bigint] NULL,
	[approved] [bit] NOT NULL,
	[handled] [bit] NULL,
	[channel_id] [bigint] NULL,
	[meter_model] [int] NULL,
	[meter_manufacturer] [int] NULL,
	[meter_type] [int] NULL
);

UPDATE	replacement.ReplacementModel
SET		handled = 1
OUTPUT  INSERTED.[command_id]
           ,INSERTED.[file]
           ,INSERTED.[meter_id]
           ,INSERTED.[address]
           ,INSERTED.[user]
           ,INSERTED.[action]
           ,INSERTED.[datetime]
           ,INSERTED.[longitude]
           ,INSERTED.[latitude]
           ,INSERTED.[old_number]
           ,INSERTED.[old_reading]
           ,INSERTED.[old_image]
           ,INSERTED.[new_number]
           ,INSERTED.[reading]
           ,INSERTED.[new_image]
           ,INSERTED.[comment]
           ,INSERTED.[code]
           ,INSERTED.[code_description]
           ,INSERTED.[diameter]
           ,INSERTED.[replacement_type]
           ,INSERTED.[consumer_id]
           ,INSERTED.[approved]
           ,INSERTED.[handled]
           ,INSERTED.[channel_id]
           ,INSERTED.[meter_model]
           ,INSERTED.[meter_manufacturer]
           ,INSERTED.[meter_type]
INTO @approved([command_id]
           ,[file]
           ,[meter_id]
           ,[address]
           ,[user]
           ,[action]
           ,[datetime]
           ,[longitude]
           ,[latitude]
           ,[old_number]
           ,[old_reading]
           ,[old_image]
           ,[new_number]
           ,[reading]
           ,[new_image]
           ,[comment]
           ,[code]
           ,[code_description]
           ,[diameter]
           ,[replacement_type]
           ,[consumer_id]
           ,[approved]
           ,[handled]
           ,[channel_id]
           ,[meter_model]
           ,[meter_manufacturer]
           ,[meter_type])
WHERE  handled = 0 AND approved = 1 AND replacement_type  IN (6, 7, 8) AND (site_id = @SiteId OR @SiteId = 0)
       AND channel_id IS NOT NULL AND consumer_id IS NOT NULL
	   AND consumer_id NOT IN (SELECT Consumer FROM ConsumerChannelHistory WHERE ToDate IS NULL);

INSERT INTO ConsumerChannelHistory(Consumer, Channel,FromDate,ToDate,UserID,MeterNumber,Diameter,meterManufacturer,meterModel,meterType,OldImage,NewImage,CommandID)
SELECT consumer_id, channel_id, [datetime], NULL, -1, new_number, diameter, meter_manufacturer,meter_model,meter_type, 
       IIF(old_image LIKE '/%', CONCAT('madr:', SUBSTRING(old_image, 2, LEN(old_image) - 1)), old_image),
       IIF(new_image LIKE '/%', CONCAT('madr:', SUBSTRING(new_image, 2, LEN(new_image) - 1)), new_image),
	   command_id
FROM   @approved AS approved;

SELECT (Payer.FirstName  + ' ' +  Payer.LastName) AS FullName,  meter_id AS MeterId,	address AS Address,	datetime AS DateTime,	longitude AS Longitude,	latitude AS Latitude,	old_number AS OldNumber,
        old_reading AS OldReading,	IIF(LEN(old_image) > 0,'=HYPERLINK(""http://reader.madrimonim.co.il/reading_images' +  old_image +  '"")',NULL) AS OldImage,	new_number AS NewNumber,	
        reading AS Reading,	IIF(LEN(old_image) > 0,'=HYPERLINK(""http://reader.madrimonim.co.il/reading_images' + new_image + '"")', NULL) AS NewImage,
        DiameterTypes.inch AS Diameter, comment AS Comment
FROM    @approved as approved
		LEFT JOIN Consumers on Consumers.ID = approved.consumer_id
		LEFT JOIN Payer on Payer.ID = Consumers.PayerID
        LEFT JOIN DiameterTypes ON DiameterTypes.ID = approved.diameter";
    }
}
