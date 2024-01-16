cd $WORKSPACE/src/PhysProj/Phys.Lib.Site.Api
version=`date -u +%Y.%m.%d.%H%M`
dotnet publish --output $WORKSPACE/cicd/envs/home/srv-app/site-api/bin /p:Version=$version