
#r "System.Net"
open System.Net
open System.IO

#r @"packages\RhinoMocks.3.6.1\lib\net\Rhino.Mocks.dll"
#r "D:\dev\FsMocks\packages\FSPowerPack.Linq.Community.3.0.0.0\Lib\Net40\FSharp.PowerPack.Linq.dll"
#load "Modules.fs"

open FsMocks
open FsMocks.Mocks
open FsMocks.Syntax2
open Rhino.Mocks

let downloadGoogleHomepage (url:string) =
    async {
        let request = HttpWebRequest.CreateHttp(url)
        use! response = Async.AwaitTask (request.GetResponseAsync())
        use responseStream = response.GetResponseStream()
        use reader = new StreamReader(responseStream)
        return reader.ReadToEnd()
        } |> Async.RunSynchronously

let downloadGoogleHomepage_fake (url:string) =
    "<html></html>"


    
Mocks.defaultFakeBuilder.Enabled <- false
let downloadAction = Mocks.defaultFakeBuilder.Build { Actual=downloadGoogleHomepage; Faked=downloadGoogleHomepage_fake}
Mocks.defaultFakeBuilder.Exec downloadAction "http://google.fr"   |> printfn "content=%s"

Mocks.defaultFakeBuilder.Enabled <- true
Mocks.defaultFakeBuilder.Exec downloadAction "http://google.fr"   |> printfn "content=%s"

