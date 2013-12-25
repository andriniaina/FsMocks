namespace Tests

open System
open Xunit
open FsMocks.Syntax
open System.Collections.Generic
open System.Windows.Forms

module Events =
    let [<Fact>] ``raise an event``() =
        let mock = FsMockRepository()
        let b:Button = mock.strict []
        let clickButtonEvent = mock.getEventRaiser (b.Click)

        mock.define Unordered {
            b.Click |> subscription expected once
        }
        mock.verify (fun () ->
            let clicked = ref false
            b.Click |> Event.add (fun arg -> clicked:=true )
            Assert.False(!clicked)
            clickButtonEvent.Raise(null, null)
            Assert.True(!clicked)
        )

