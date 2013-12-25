namespace Tests

open System
open System.Windows.Forms
open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module withDefaultValues = 
    let [<Fact>] withDefaultValues() =
        let mock = FsMockRepository()
        let o:Control = mock.withDefaultValues []
        mock.define Unordered {
            ()  // no expectation : it's a dynamic mock : everything is allowed, all functions/properties are empty and return default values
        }
        mock.verify (fun() ->
            // property set should do nothing
            o.Text <- "u"
            Assert.Null(o.Text)
            // call any virtual method : we should raise no error
            o.ResetText()
            o.Refresh()
            // ...or return a default value
            Assert.Equal(new Drawing.Size(), o.GetPreferredSize(Drawing.Size.Empty))
        )

