namespace WebSharperSuave

open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Server

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/about">] About

module Templating =
    open WebSharper
    open WebSharper.UI.Next.Html
    open WebSharper.UI.Next.Templating

    type Templates = Template<
                        "views/Main.html,views/Navbar.html",
                        serverLoad=ServerLoad.WhenChanged>
    
    // Compute a menubar where the menu item for the given endpoint is active
    let navbar (ctx : Context<EndPoint>) endpoint=
        let ( => ) (txt : string) act =
            let liClass = if endpoint = act then "active" else System.String.Empty
            Templates
                .Navbar
                .NavbarItem()
                .liClasses(liClass)
                .aHref(ctx.Link act)
                .Text(txt)
                .Doc()

        Templates
            .Navbar()
            .NavbarList(
            [
                "Home" => EndPoint.Home
                "About" => EndPoint.About
            ])
            .Doc()

    let Main ctx action (title : string) (body : Doc list) =
        let resources = [
            Doc.WebControl(new Web.Require<Resources.MainStyle>())
            Doc.WebControl(new Web.Require<Resources.Bootstrap>())
        ]

        Content.Page(
            Templates
                .Main()
                .Title(title)
                .Navbar(navbar ctx action)
                .Body(resources @ body)
                .Doc()
        )

module Site =
    open WebSharper
    open WebSharper.UI.Next.Html

    let homePage ctx =
        Templating.Main ctx EndPoint.Home "Home" [
            h1 [text "Say Hi to the server!"]
            client <@ Client.Main() @>
        ]

    let aboutPage ctx =
        Templating.Main ctx EndPoint.About "About" [
            h1 [text "About"]
            p [text "This is a template WebSharper client-server application."]
        ]

    let main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> homePage ctx
            | EndPoint.About -> aboutPage ctx
        )


module Main =
    open Suave
    open Suave.Filters
    open Suave.Operators
    open System
    open System.IO
    open System.Reflection
    open WebSharper.Suave
    
    let codeBase = Assembly.GetEntryAssembly().CodeBase
    let builder = UriBuilder(codeBase)
    let pathToAssembly = Path.GetDirectoryName (Uri.UnescapeDataString(builder.Path))
    let rootPath = Path.GetFullPath(Path.Combine(pathToAssembly, "../"))
    // let debugConfig = { defaultConfig with logger = Loggers.saneDefaultsFor LogLevel.Verbose }
    let config = { defaultConfig with homeFolder = Some (Path.Combine [|rootPath; "public"|]) }
    let assets = choose [ Files.browseHome; RequestErrors.NOT_FOUND "Page not found." ]

    do startWebServer config (WebSharperAdapter.ToWebPart (Site.main, RootDirectory=rootPath, Continuation=assets))
