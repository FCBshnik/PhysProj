cd $JENKINS_HOME
eval "$(ssh-agent)"
ssh-add physproj-home-app
cd $WORKSPACE/cicd/envs/home/srv-app
sh './admin-web/deploy.sh'