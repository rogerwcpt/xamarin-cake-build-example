///////////////////////////////////////////////////////////////////////////////
// Example usage:  
// Windows: build.ps1 -configuration=Release
// OSX : ./build.sh -configuration=Release
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////

public MSBuildSettings CreateDefaultBuildSettings(string buildConfig, string outputPath, string solutionDir)
{
    var result = new MSBuildSettings();
    result.Verbosity = Verbosity.Minimal;
    result.Configuration = buildConfig;
    result.WithTarget("Clean");
    result.WithTarget("Build");   
    result.WithProperty("OutputPath", outputPath);
 	 result.WithProperty("SolutionDir", solutionDir);
    return result;
}

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("PackageAndroid")
    .Does(()=>
    {
      var solutionDir = ".";
      var androidProject = "./Droid/XamarinApp.Droid.csproj";
      var outputPath = "bin/" + configuration + "/";
      var absolutePath = MakeAbsolute(Directory(solutionDir)).ToString();

      var buildSettings = CreateDefaultBuildSettings(configuration, outputPath, solutionDir);

      buildSettings.WithProperty("AndroidUseSharedRuntime",  "false" ); //Don't use shared mono runtime
      buildSettings.WithProperty("EmbedAssembliesIntoApk", "true"); //True = Fast Deployment is OFF
      buildSettings.WithProperty("BundleAssemblies", "true"); // True = Embed Assemblies into Native Code
      buildSettings.WithProperty("AotAssemblies", "true"); // Enable AOT - doubles the package size, but much faster startup times
      buildSettings.WithProperty("EnableLLVM", "true"); // Enable LLVM Optmizing copmiler
  
      buildSettings.WithProperty("AndroidLinkMode", "SdkOnly"); // Link Behavior

      buildSettings.WithProperty("AndroidSigningKeyStore", absolutePath + "/Droid/Keys/debug.keystore");
      buildSettings.WithProperty("AndroidSigningKeyAlias", "AndroidDebugKey");
      buildSettings.WithProperty("AndroidSigningStorePass", "android");
      buildSettings.WithProperty("AndroidSigningKeyPass", "android");         

      buildSettings.Targets.Clear();
      buildSettings.WithTarget("SignAndroidPackage");
       
      // packageSettings.WithProperty("AndroidSupportedAbis", "armeabi-v7a%3Bx86"); // Need to encode the semicolon as %3B. Use this line instead if you want to add x86 emulator support for the APK
      buildSettings.WithProperty("AndroidSupportedAbis", "armeabi-v7a");
      
      MSBuild(androidProject, packageSettings);
    });

Task("Default")
   .IsDependentOn("PackageAndroid")
   .Does(() => {
   });

RunTarget(target);