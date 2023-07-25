cd $WORKSPACE/src/PhysProj/Phys.Lib.Api.Admin
version=`date -u +%Y.%m.%d.%H%M`
dotnet publish --output $WORKSPACE/envs/home/srv-app/admin-api/bin /p:Version=$version