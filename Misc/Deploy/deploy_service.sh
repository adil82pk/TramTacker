echo "Backing up existing deployment..."

if [ ! -d "/c/havm2tramtracker_source_deploy" ]
then
    mkdir -p /c/havm2tramtracker_source_deploy/previous_versions/
fi

cp -r /c/havm2tramtracker /c/havm2tramtracker_source_deploy/previous_versions/havm2tramtracker$(date +%Y%m%d_%H%M%S)

echo "Checking that repo is available"
if [ -d "/c/havm2tramtracker_source_deploy/havm2tramtracker" ]
then
	echo "ok"
else
    cd /c/havm2tramtracker_source_deploy
	git clone https://iopdeploy:<password here>@bitbucket.org/ytavmis/havm2tramtracker.git
fi

echo "Getting the latest from TBU-38-downgrade-havm2tramtracker-to-c-5 branch"
cd /c/havm2tramtracker_source_deploy/havm2tramtracker
git checkout feature/TBU-38-downgrade-havm2tramtracker-to-c-5
git pull origin feature/TBU-38-downgrade-havm2tramtracker-to-c-5

cd /c/havm2tramtracker_source_deploy

echo "Building Havm2TramTracker Service..."
cmd "/c restore_packages.bat"
cmd "/c build_service.bat"

cd /c/havm2tramtracker
echo "Moving service config file"
cp ./YarraTrams.havm2tramtracker.Processor.exe.config ./bin

echo "Press any key to continue"
read