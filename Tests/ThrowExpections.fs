namespace Tests

open System
open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module ThrowExpections =
    let [<Fact>] ``expect an exception``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> throws (new ApplicationException())
        }
        
        mock.verify (fun() ->
            Assert.Throws<ApplicationException>(fun () -> list.Clear())
            |> ignore
            )
    

