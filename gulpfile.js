const { src, dest, parallel, series, watch } = require('gulp');

//const twig = require('gulp-twig');
//const sass = require('gulp-sass');
//const prefix = require('gulp-autoprefixer');
//const data = require('gulp-data');
//const sourcemaps = require('gulp-sourcemaps');
//const concat = require('gulp-concat');
//const plumber = require('gulp-plumber');
//const browsersync = require('browser-sync');
//const gulpcopy = require('gulp-copy');
//const fs = require('fs');
//const del = require('del');
//const path = require('path');

//const gulp = require('gulp');
/// <binding Clean='Build-Solution' />
const msbuild = require("gulp-msbuild");
const debug = require("gulp-debug");
const foreach = require("gulp-foreach");
const rename = require("gulp-rename");
const newer = require("gulp-newer");
const util = require("gulp-util");
const runSequence = require("run-sequence");
const fs = require("fs");
const yargs = require("yargs").argv;
const path = require("path");
const rimrafDir = require("rimraf");
const rimraf = require("gulp-rimraf");
const xmlpoke = require("xmlpoke");
const unicorn = require("./scripts/unicorn.js");
const habitat = require("./scripts/habitat.js");
const helix = require("./scripts/helix.js");

var config;
if (fs.existsSync("./gulp-config.js.user")) {
    config = require("./gulp-config.js.user")();
} else {
    config = require("./gulp-config.js")();
}

module.exports.config = config;

/*****************************
  Initial setup
*****************************/
var publishProjects = function (location, dest) {
    dest = dest || config.websiteRoot;

    console.log("publish to " + dest + " folder");
    return src([location + "/**/code/*.csproj"])
        .pipe(foreach(function (stream, file) {
            return publishStream(stream, dest);
        }));
};
///*****************************
//  Publish
//*****************************/
var publishStream = function (stream, dest) {
    var targets = ["Build"];

    return stream
        .pipe(debug({ title: "Building project:" }))
        .pipe(msbuild({
            targets: targets,
            configuration: config.buildConfiguration,
            logCommand: false,
            verbosity: config.buildVerbosity,
            stdout: true,
            errorOnFail: true,
            maxcpucount: config.buildMaxCpuCount,
            nodeReuse: false,
            toolsVersion: config.buildToolsVersion,
            properties: {
                Platform: config.publishPlatform,
                DeployOnBuild: "true",
                DeployDefaultTarget: "WebPublish",
                WebPublishMethod: "FileSystem",
                BuildProjectReferences: "false",
                DeleteExistingFiles: "false",
                publishUrl: dest
            }
        }));
};


function PublishAllProjects(done) {
    //series(buildSolution, parallel(publishFeature, publishFoundation, publishProject))(done);
    series(parallel(PublishAssemblies, PublishAllViews, PublishAssets, PublishAllConfigs))(done);
}

function buildSolution() {
    var targets = ["Build"];
    if (config.runCleanBuilds) {
        targets = ["Clean", "Build"];
    }
    var solution = "./" + config.solutionName + ".sln";
    return src(solution)
        .pipe(msbuild({
            targets: targets,
            configuration: config.buildConfiguration,
            logCommand: false,
            verbosity: config.buildVerbosity,
            stdout: true,
            errorOnFail: true,
            maxcpucount: config.buildMaxCpuCount,
            nodeReuse: false,
            toolsVersion: config.buildToolsVersion,
            properties: {
                Platform: config.buildPlatform
            },
            customArgs: ["/restore"]
        }));
}
function publishFoundation() {
    return publishProjects("./Src/Foundation");
}
function publishFeature() {
    return publishProjects("./Src/Feature");
}
function publishProject() {
    return publishProjects("./Src/Project");
}


///*****************************
//  CI Publish
//  Credits to James - Inspired from : https://www.jameshirka.com/2016/10/sitecore-habitat-deployment/
//*****************************/

function CIBuild(done) {
    config.websiteRoot = path.resolve("./Output");
    config.runCleanBuilds = true;
    //config.buildConfiguration = "Release";
    //fs.mkdirSync(config.websiteRoot);
    series(CIClean, parallel(CIPublish))(done);
}
function CIPublish(done) {
    config.websiteRoot = path.resolve("./Output");
    config.runCleanBuilds = true;
    //config.buildConfiguration = "Release";
    fs.mkdirSync(config.websiteRoot);
    series(buildSolution, parallel(PublishAssemblies, PublishAllViews, PublishAssets, PublishAllConfigs))(done);

}

function CIClean(done) {
    rimrafDir.sync(path.resolve("./Output"));
    done();
}

function CICopyItems() {

    return src('./Src/**/serialization/**/*.yml')
        // .other plugins
        .pipe(dest("./Output/Data/unicorn"));
}

function SyncUnicorn(done) {
    var options = {};
    options.siteHostName = habitat.getSiteUrl();
    options.authenticationConfigFile = config.websiteRoot + "/App_config/Include/Unicorn.SharedSecret.config";

    unicorn(function () { return done() }, options);
}

/* Remove unwanted files */
function PreparePackageFiles() {
    var excludeList = [
        //config.websiteRoot + "\\bin\\{Sitecore,Lucene,Newtonsoft,System,Microsoft.*}*{dll,pdb}",
        config.websiteRoot + "\\bin\\{de,es,fr,it,ja,ko,roslyn,ru,zh-Hans,zh-Hant}",
        //config.websiteRoot + "\\App_Config\\Include\\{Feature,Foundation,Project}\\*Serialization.config",
        //config.websiteRoot + "\\App_Config\\Include\\{Feature,Foundation,Project}\\z.*DevSettings.config",
        config.websiteRoot + "\\bin\\*{dll,pdb,dll.config}",
        "!" + config.websiteRoot + "\\bin\\{ENBDGroup.}*dll",
        //"!" + config.websiteRoot + "\\bin\\Sitecore.{Feature,Foundation,Habitat,Demo,Common}*dll"
    ];
    console.log(excludeList);

    return src(excludeList, { read: false }).pipe(rimraf({ force: true }));
}

function PublishAssemblies() {
    var root = "./Src";
    var binFiles = root + "/**/website/**/bin/{HtmlSanitizer,AngleSharp,Hammock.ClientProfile,ENBDGroup.}*.dll";
    var destination = config.websiteRoot + "/bin/";
    return src(binFiles, { base: root })
        .pipe(rename({ dirname: "" }))
        .pipe(newer(destination))
        .pipe(debug({ title: "Copying " }))
        .pipe(dest(destination));
}
function PublishAllConfigs() {
    var root = "./Src";
    var roots = [root + "/**/App_Config", "!" + root + "/**/tests/App_Config", "!" + root + "/**/obj/**/App_Config"];
    var files = "/**/ENBDGroup.*.config";
    var destination = config.websiteRoot + "\\App_Config";
    return src(roots, { base: root }).pipe(
        foreach(function (stream, file) {
            console.log("Publishing from " + file.path);
            src(file.path + files, { base: file.path })
                .pipe(newer(destination))
                .pipe(debug({ title: "Copying " }))
                .pipe(dest(destination));
            return stream;
        })
    );
}
function PublishAllViews() {
    var root = "./Src";
    var roots = [root + "/**/Views", "!" + root + "/**/obj/**/Views"];
    var files = "/**/*.cshtml";
    var destination = config.websiteRoot + "\\Views";
    return src(roots, { base: root }).pipe(
        foreach(function (stream, file) {
            console.log("Publishing from " + file.path);
            src(file.path + files, { base: file.path })
                .pipe(newer(destination))
                .pipe(debug({ title: "Copying " }))
                .pipe(dest(destination));
            return stream;
        })
    );
}

function PublishAssets() {
    var root = "./Src";
    var roots = [root + "/**/assets", "!" + root + "/**/obj/**/assets"];
    var files = "/**/*.*";
    var destination = config.websiteRoot + "\\assets";
    return src(roots, { base: root }).pipe(
        foreach(function (stream, file) {
            console.log("Publishing from " + file.path);
            src(file.path + files, { base: file.path })
                .pipe(newer(destination))
                .pipe(debug({ title: "Copying " }))
                .pipe(dest(destination));
            return stream;
        })
    );
}
//
exports.default = PublishAllProjects;
exports.SyncUnicorn = SyncUnicorn;
exports.CIBuild = CIBuild;
exports.CIClean = CIClean;
exports.PreparePackageFiles = PreparePackageFiles;
exports.PublishAssemblies = PublishAssemblies;
exports.PublishAllConfigs = PublishAllConfigs;
exports.PublishAllViews = PublishAllViews;
exports.PublishAssets = PublishAssets;