nuget restore
msbuild CoreBot.sln -p:DeployOnBuild=true -p:PublishProfile=.\weatherchatbotapp-Web-Deploy.pubxml -p:Password=pYcj84cTRvrPiGroZZJphuRslRDc85mj4alQ0hxcdvTHguFeXrseK0ne4Log

