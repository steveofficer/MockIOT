open System
open System.IO
open System.Net.Http
open FSharp.Control
open FSharp.Control.Tasks

[<EntryPoint>]
let main argv =
    use client = new HttpClient()
    Headers.MediaTypeWithQualityHeaderValue("text/event-stream") |> client.DefaultRequestHeaders.Accept.Add

    let connect() = task {
        printfn "Establishing connection"
        
        let! source = client.GetStreamAsync("http://localhost:5000/api/device1")
        use reader = new StreamReader(source)
        
        printfn "Connected"

        do!
            asyncSeq {
                while not reader.EndOfStream do
                    let! data = reader.ReadLineAsync() |> Async.AwaitTask
                    yield data
            } 
            |> AsyncSeq.filter (fun s -> s |> System.String.IsNullOrEmpty |> not) 
            |> AsyncSeq.iterAsync (fun s -> async { printfn "%s" s })
    }

    connect().GetAwaiter().GetResult()
    
    0 // return an integer exit code
