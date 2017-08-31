namespace WebSharperSuave

module Resources =
    open WebSharper
    open WebSharper.Core.Resources

    type MainStyle() =
        inherit BaseResource("/assets/css/main.css")

    [<Require(typeof<JQuery.Resources.JQuery>)>]
    type Bootstrap() =
        inherit BaseResource (
                "https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6",
                "css/bootstrap.min.css",
                "js/bootstrap.min.js"
            )