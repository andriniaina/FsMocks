namespace Tests

open System
open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module ThrowExpections =
    let [<Fact>] ``expect an exception``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> throws (new ApplicationException())
        }
        
        Assert.Throws<ApplicationException>(fun () -> list.Clear())
        |> ignore
    

