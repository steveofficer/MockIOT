open System
open System.IO
open System.Net.Http

[<EntryPoint>]
let main argv =
    use client = new HttpClient()
    Headers.MediaTypeWithQualityHeaderValue("text/event-stream") |> client.DefaultRequestHeaders.Accept.Add

    let connect() = 
        printfn "Establishing connection"
        
        use reader = new StreamReader(client.GetStreamAsync("http://localhost:5000/api/device1") |> Async.AwaitTask |> Async.RunSynchronously)
        
        printfn "Connected"

        let rec readNext() =
            match reader.EndOfStream with
            | true -> ()
            | _ -> 
                reader.ReadLine() |> printfn "received - %s" 
                readNext()

        readNext()
    
    connect()
    0 // return an integer exit code
