namespace WSSuave

open WebSharper

module Server =

    [<Rpc>]
    let DoSomething input =
        let reverse (s: string) = System.String(Array.rev(s.ToCharArray()))
        async {
            let reversed = reverse input
            printfn "input: %s  -  reverse: %s" input reversed
            return reversed
        }
