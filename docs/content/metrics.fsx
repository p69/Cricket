(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin"
#r "FSharp.Actor.dll"
open FSharp.Actor
open System.Threading

(**

Metrics
=======

*)

ActorHost.Start(fun c -> 
    { c with 
        Metrics = Some {
            Metrics.Configuration.Default with
                ReportCreator = Metrics.WriteToFile(5000, @"C:\temp\Metrics.txt", Metrics.Formatters.toString)
        }
    })

type Say =
    | Hello
    | HelloWorld
    | Name of string

let greeter = 
    actor {
        name "greeter"
        messageHandler (fun actor ->
            let rec loop() = async {
                let! msg = actor.Receive() //Wait for a message
//                match msg.Message with
//                | Hello ->  actor.Logger.Debug("Hello") //Handle Hello leg
//                | HelloWorld -> actor.Logger.Debug("Hello World") //Handle HelloWorld leg
//                | Name name -> actor.Logger.Debug(sprintf "Hello, %s" name) //Handle Name leg
                return! loop() //Recursively loop

            }
            loop())
    } |> Actor.spawn

let cts = new CancellationTokenSource()

let rec publisher() = async {
    do greeter <-- Name "Metrics"
    return! publisher()
}

Async.Start(publisher(), cts.Token)
cts.Cancel()

greeter <-- Shutdown