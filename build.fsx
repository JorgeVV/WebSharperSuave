// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"
open Fake

module Logg =
    open System
    let private writeLine color (text : string) =
        Console.ForegroundColor <- color
        Console.WriteLine text
        Console.ResetColor ()

    let ok = writeLine ConsoleColor.Green
    let info = writeLine ConsoleColor.Cyan
    let debug = writeLine ConsoleColor.Magenta
    let warn = writeLine ConsoleColor.Yellow
    let error = writeLine ConsoleColor.Red
    let other = writeLine ConsoleColor.White

// Directories
[<Literal>]
let BuildDir  = "./build/"
[<Literal>]
let DeployDir = "./deploy/"


// Filesets
let appReferences  =
    !! "/**/*.csproj"
    ++ "/**/*.fsproj"

// Version info
[<Literal>]
let Version = "0.1"  // or retrieve from CI server

// App name
[<Literal>]
let AppName = "WebSharperSuave"

let kill _ =
    Async.CancelDefaultToken ()
    killAllCreatedProcesses ()
    //killProcess AppName

let clean _ = CleanDirs [BuildDir; DeployDir]

let binPath = BuildDir @@ "_PublishedWebsites" @@ AppName @@ "bin"

let build _ = 
    let configPath = BuildDir @@ AppName + ".exe.config"
    let buildFiles = !! (BuildDir @@ "*.*")

    // compile all projects below src/app/
    MSBuildDebug BuildDir "Build" appReferences
    |> Log "AppBuild-Output: "

    CopyFile binPath configPath
    DeleteFiles buildFiles

let run _ =
    let appExe = AppName + ".exe"
    Logg.info "About to run process. Press 'Q' to quit it once started."
    async {
        return! asyncShellExec {
            Program = binPath @@ appExe
            WorkingDirectory = binPath
            CommandLine = System.String.Empty
            Args = []
        }
    } |> Async.Ignore |> Async.Start

// Targets
Target "Clean" clean

Target "Build" build

Target "Deploy" (fun _ ->
    !! (BuildDir @@ "/**/*.*")
    -- "*.zip"
    |> Zip BuildDir (DeployDir + AppName + "." + Version + ".zip")
)

Target "Watch" (fun _ ->
    let tasks =
        kill
//        >> clean
        >> build
        >> run

    use watcher =
        !! "src/**/*.fs" ++ "src/**/*.fsproj" ++ "src/**/*.config"
        |> WatchChanges (fun changes ->
            Logg.warn "Changes detected:"
            changes
            |> Seq.iter (fun change ->
                match change.Status with
                | Created -> Printf.kprintf Logg.ok "-> Created: %s." change.FullPath
                | Changed -> Printf.kprintf Logg.info "-> Edited: %s." change.FullPath
                | Deleted -> Printf.kprintf Logg.error "-> Deleted: %s." change.FullPath
            )
            Logg.info "Building..."
            tasks ()
        )

    kill ()
    run ()

    let readChar () = (System.Console.ReadKey true).KeyChar
    let rec loop = function
        | 'q' | 'Q' -> watcher.Dispose ()
        | _         -> loop (readChar ())

    readChar () |> loop
)

// Build order
"Clean"
  ==> "Build"
  ==> "Watch" <=> "Deploy"

// start build
RunTargetOrDefault "Build"
