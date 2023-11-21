cd $WORKSPACE/src/PhysProj/Phys.Lib.Site.Web
npm install
npm run build
cp -r ./build/ $WORKSPACE/envs/home/srv-app/site-web