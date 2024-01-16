cd $WORKSPACE/src/PhysProj/Phys.Lib.Admin.Api
version=`date -u +%Y.%m.%d.%H%M`
dotnet publish --output $WORKSPACE/cicd/envs/home/srv-app/admin-api/bin /p:Version=$version