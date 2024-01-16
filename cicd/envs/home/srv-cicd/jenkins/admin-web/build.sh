cd $WORKSPACE/src/PhysProj/Phys.Lib.Admin.Web
npm install
npm run build
cp -r ./build/ $WORKSPACE/cicd/envs/home/srv-app/admin-web