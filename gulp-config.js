module.exports = function () {
    var instanceRoot = "C:\\inetpub\\wwwroot\\enbddevcm.dev.local";
    var config = {
        websiteRoot: instanceRoot + "\\",
        sitecoreLibraries: instanceRoot + "\\bin",
        //licensePath: instanceRoot + "\\App_Data\\license.xml",
        packagePath: instanceRoot + "\\App_Data\\packages",
        solutionName: "ENBDGroup",
        buildConfiguration: "Debug",
        buildToolsVersion: "auto",
        buildToolsVersion: 15.0,
        buildMaxCpuCount: 0,
        buildVerbosity: "minimal",
        buildPlatform: "Any CPU",
        publishPlatform: "AnyCpu",
        runCleanBuilds: true
    };
    return config;
}


