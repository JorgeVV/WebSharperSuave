namespace WebSharperSuave

module Resources =
    open WebSharper
    open WebSharper.Core.Resources

    type MainStyle() =
        inherit BaseResource("/css/main.css")

    [<Require(typeof<JQuery.Resources.JQuery>)>]
    type Tether() =
        inherit BaseResource("https://cdnjs.cloudflare.com/ajax/libs/tether/1.4.0/js/tether.min.js")

    [<Require(typeof<JQuery.Resources.JQuery>)>]
    [<Require(typeof<Tether>)>]
    type Bootstrap() =
        inherit BaseResource (
                "https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6",
                "css/bootstrap.min.css",
                "js/bootstrap.min.js"
            )
