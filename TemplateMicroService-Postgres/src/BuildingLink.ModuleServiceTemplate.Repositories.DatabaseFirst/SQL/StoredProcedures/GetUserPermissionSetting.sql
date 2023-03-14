CREATE   PROCEDURE [dbo].[GetUserPermissionSetting] 
	@userId int, 
	@permissionId int
AS
BEGIN
	SET NOCOUNT ON;

DECLARE
  @OccupancyId    INT,
  @PhysicalUnitId INT,
  @SubTypeNode    HIERARCHYID,
  @BuildingId     INT,
  @MgmtCompanyId  INT,
  @BuildingTypeId INT;

SELECT
    @OccupancyId = u.UnitID,
    @PhysicalUnitId = o.PhysicalUnitID,
    @SubTypeNode = ust.Node,
    @BuildingId = u.FacID,
    @MgmtCompanyId = COALESCE(b.MAID, mcs.ManagingAgencyId),
    @BuildingTypeId = b.CRMBuildingTypeId
FROM tblUser AS u WITH (NOLOCK)
    LEFT JOIN UserSubType AS ust WITH (NOLOCK) ON ust.Id = u.SubTypeId
    LEFT JOIN tblUnits AS o WITH (NOLOCK) ON o.UnitID = u.UnitID
    LEFT JOIN tblFacilities AS b WITH (NOLOCK) ON b.FacID = u.FacID
    LEFT JOIN ManagementCompanySuperuser AS mcs WITH (NOLOCK) ON mcs.UserId = u.UserID
WHERE u.UserID = @UserId;


DECLARE @options TABLE (
  PermissionId SMALLINT NOT NULL,
  BuildingTypeId INT NOT NULL,
  ManagingAgencyId INT NOT NULL,
  BuildingId INT NOT NULL,
  UserTypeId INT NOT NULL,
  PhysicalUnitId INT NOT NULL,
  OccupancyId INT NOT NULL,
  UserId INT NOT NULL,
  OptionId SMALLINT NOT NULL,
  CreateDateUTC DATETIME NOT NULL,
  ChangeDateUTC DATETIME NOT NULL,
  CreateUserId INT NOT NULL,
  ChangeUserId INT NOT NULL,
  SubTypeId INT NOT NULL,
  NodeLevel SMALLINT NULL);

INSERT INTO @options
SELECT
    ps.PermissionId,
    ps.BuildingTypeId,
    ps.ManagingAgencyId,
    ps.BuildingId,
    ps.UserTypeId,
    ps.PhysicalUnitId,
    ps.OccupancyId,
    ps.UserId,
    ps.OptionId,
    ps.CreateDateUTC,
    ps.ChangeDateUTC,
    ps.CreateUserId,
    ps.ChangeUserId,
    ps.SubTypeId,
    pust.NodeLevel
FROM dbo.PermissionSetting AS ps WITH (NOLOCK)
    INNER JOIN dbo.UserSubType AS pust WITH (NOLOCK) ON pust.Id = ps.SubTypeId
WHERE ps.PermissionId = @PermissionId
    AND ps.UserId IN (1, @UserId)
    AND ps.OccupancyId IN (1, @OccupancyId)
    AND ps.PhysicalUnitId IN (1, @PhysicalUnitId)
    AND (ps.SubTypeId = 1 OR @SubTypeNode.IsDescendantOf(pust.Node) = 1)
    AND ps.BuildingId IN (1, @BuildingId)
    AND ps.ManagingAgencyId IN (1, @MgmtCompanyId)
    AND ps.BuildingTypeId IN (1, @BuildingTypeId);

SELECT TOP(1)
  PermissionId,
  BuildingTypeId,
  ManagingAgencyId,
  BuildingId,
  UserTypeId,
  PhysicalUnitId,
  OccupancyId,
  UserId,
  OptionId,
  CreateDateUTC,
  ChangeDateUTC,
  CreateUserId,
  ChangeUserId,
  SubTypeId
FROM @options
ORDER BY
    CASE WHEN UserId <> 1 THEN 0 ELSE 1 END,
    CASE WHEN OccupancyId <> 1 THEN 0 ELSE 1 END,
    CASE WHEN PhysicalUnitId <> 1 THEN 0 ELSE 1 END,
    CASE WHEN SubTypeId <> 1 AND BuildingId <> 1 THEN NodeLevel ELSE -1 END DESC,
    CASE WHEN BuildingId <> 1 THEN 0 ELSE 1 END,
    CASE WHEN SubTypeId <> 1 THEN NodeLevel ELSE -1 END DESC,
    CASE WHEN ManagingAgencyId <> 1 THEN 0 ELSE 1 END,
    CASE WHEN BuildingTypeId <> 1 THEN 0 ELSE 1 END;

END