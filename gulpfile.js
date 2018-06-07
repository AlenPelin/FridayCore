var gulp = require("gulp");
var msbuild = require("gulp-msbuild");
var nugetRestore = require("gulp-nuget-restore");
var replace = require('gulp-replace'); 
var nuget = require("gulp-nuget");
var fs = require("fs");
var del = require("del");
var runSequence = require("run-sequence");

var solution = "./src/FridayCore.sln";
var nugetPath = "./src/.nuget/nuget.exe";

gulp.task("default", function (callback) {
    return runSequence(
        "1-Restore",
        "2-Build",
        "3-Pack",
        callback);
});

gulp.task("1-Restore", function (callback) {
    return runSequence("restore", callback);
});

gulp.task("2-Build", function (callback) {
    return runSequence("build", callback);
});

gulp.task("3-Pack", function (callback) {
    return runSequence("pack", callback);
});

gulp.task("restore", function () {
    return gulp.src(solution).pipe(nugetRestore());
});

gulp.task("build", function (callback) {
    return runSequence(
        "updateVersionInfoCs",
        "msbuild",
        "revertVersionInfoCs",
        callback
    );
});

gulp.task("msbuild", function(){ 
    var targets = ["Clean", "Build"];
    
    return gulp.src(solution)
        .pipe(msbuild({
            targets: targets,
            configuration: "Release",
            logCommand: false,
            verbosity: "minimal",
            stdout: true,
            errorOnFail: true,
            maxcpucount: 0,
            toolsVersion: 15.0,
            properties: {
                Platform: "Any CPU"
            }
        }));
});

gulp.task("updateVersionInfoCs", function(){
    updateVersionInfCs(getVersion());
});

gulp.task("revertVersionInfoCs", function(){
    updateVersionInfCs("0.0.0.0");
});

gulp.task("pack", function(callback) {
    return runSequence(
        "updateNuspec",
        "nugetPack",
        "revertNuspec",
        callback);
});

gulp.task("updateNuspec", function(){
    updateNuspecFiles(getVersion());
});

gulp.task("nugetPack", function(){
    del("./release");

    return gulp.src(["./src/FridayCore*/**/*.nuspec"])
        .pipe(nuget.pack({ nuget: nugetPath, version: getVersion() }))
        .pipe(gulp.dest("./release")); 
});

gulp.task("revertNuspec", function(){
    updateNuspecFiles('0.0.0.0'); 
});

function updateNuspecFiles(version){
    return gulp.src("./src/FridayCore*/**/*.nuspec")
        .pipe(replace(/(\s*)\<dependency\s+id\s*\=\s*\"FridayCore(.+)"\s+version\s*=\s*"[^\"]+"\s*\/\>/g, '$1<dependency id="FridayCore$2" version="' + version + '" />'))
        .pipe(gulp.dest("./src/")); 
}

function updateVersionInfCs(version){    
    fs.writeFileSync("./src/FridayCore/Properties/VersionInfo.cs",
        "// DO NOT CHANGE\r\n" +
        "\r\n" +
        "[assembly: System.Reflection.AssemblyVersion             (\"" + version + "\")]\r\n" + 
        "[assembly: System.Reflection.AssemblyFileVersion         (\"" + version + "\")]\r\n" + 
        "[assembly: System.Reflection.AssemblyInformationalVersion(\"" + version + "\")]\r\n");
}

function getVersion() {
    var version;
    if (fs.existsSync(".version")) {
        version = new String(fs.readFileSync(".version"));
    } else {
        var line = new String(fs.readFileSync("./.appveyor.yml"));
        line = line.indexOf('\n') > 0 ? line.substring(0, line.indexOf('\n')) : line;
        line = line.indexOf('\r') > 0 ? line.substring(0, line.indexOf('\r')) : line;
        line = line.trim();
        version = line.substring(line.indexOf(':') + 1).replace('.{build}', '.0');
    }

    version = version.trim();
    console.log("Gulp detected version: '" + version + "'");

    return version;
}