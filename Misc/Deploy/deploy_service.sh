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

echo "Getting the latest from dev branch"
cd /c/havm2tramtracker_source_deploy/havm2tramtracker
git checkout dev
git pull origin dev

cd /c/havm2tramtracker_source_deploy

echo "Restoring Havm2TramTracker packages..."
cmd "/c restore_packages.bat"
echo "Building Havm2TramTracker Service..."
cmd "/c build_service.bat"
echo "Building Havm2TramTracker Console..."
cmd "/c build_console.bat"

echo "Running TramTracker database migrations..."
cd /c/havm2tramtracker_source_deploy/havm2tramtracker/YarraTrams.Havm2TramTracker/YarraTrams.Havm2TramTracker.Processor/Migrations
cmd "/c TTBU_migrations_up.bat <database server name here> TramTracker true "

cd /c/havm2tramtracker
echo "Removing all configs that the build copied across"
rm ./bin/*.config
rm ./consolebin/*.config
echo "Moving service config file"
cp ./YarraTrams.havm2tramtracker.Processor.exe.config ./bin
echo "Moving console config file"
cp ./YarraTrams.havm2tramtracker.Console.exe.config ./consolebin

echo "Press any key to continue"
read