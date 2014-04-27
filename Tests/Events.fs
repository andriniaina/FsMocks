namespace Tests

open System
open Xunit
open FsMocks.Syntax
open System.Collections.Generic
open System.Windows.Forms

module Events =
    let [<Fact>] ``manually raise an event``() =
        use mock = new FsMockRepository()
        let b:Button = mock.strict []
        let clickButtonEvent = mock.getEventRaiser (b.Click)

        mock.define Unordered {
            b.Click |> subscription expected once
        }

        let clicked = ref false
        b.Click |> Event.add (fun arg -> clicked:=true )
        Assert.False(!clicked)
        clickButtonEvent.Raise(null, null)
        Assert.True(!clicked)

