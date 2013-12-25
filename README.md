FsMocks / FsharpMocks
=======
F# mock library
---------------------


FsMocks is an object mocking library written in F#. It is actually a wrapper around [Rhino.Mocks](http://ayende.com/wiki/Rhino+Mocks+Documentation.ashx) that simplifies mocking with F#. 
The API is simple and straightforward because it uses a human-friendly DSL syntax. 
it can be combined with other test frameworks (NUnit, xUnit, FsUnit, etc.)

Samples
---------------------
This sample creates a new IList and expects calls to the methods _Add()_ _Clear()_ _Contains()_, in any order, but with some constraints.

```fsharp
open System.Collections
// create mock object and repository
let mock = FsMockRepository()
let mylist1:IList = mock.strict []

// define mock statements
mock.define Unordered {
  mylist1.Contains(1) |> returns false |> only_if_argument [Is.LessThanOrEqual(300)]
  mylist1.Add "e" |> expected at_any_moment |> only_if_argument [Is.NotNull()]
  mylist1.Clear() |> expected twice
}

// test and verify expectations !!
// the test will fail if any unexpected calls was made in the 'verify' block
// FsMocks can be combined with any test framework (NUnit, xUnit, FsUnit, etc.)
mock.verify (fun()->
	mylist1.Add("unknown argument") |> should equal 1  // FsUnit syntax
	mylist1.Clear()
	mylist1.Clear()
	mylist1.Add("another argument") |> ignore
)
```

The function _mock.define_ takes two arguments: a mocking option (_Unordered_ or _Ordered_) and a computation expression that takes a series of mock statements.

A mock statement is a unit expression that begins with the call or property to mock followed by mock directives:
```fsharp
// the call is expected twice
o.call() |> expected twice 

// the mock object will return 219 when this expectation is satisfied
o.call("some arg") |> returns 219

// the call is expected only if it respects the given constraints
o.call(arg) |> only_if_argument [Is.NotNull()] 

// Property1 will be implemented as a simple get/set property
o.Property1 |> implement_as_property

// the most powerful statement : the call is manually implemented
let myCustomClearImplementation() = System.Console.WriteLine("list cleared!!!")
list.Clear() |> implement_as (new Action(myCustomClearImplementation))

// throws an exception whenever a method is called
o.call() |> throws (new Exception("Something went wrong!!"))

// subscribe to an event and simulate an event
let b:Button = mock.strict []
let clickRaiser = mock.getEventRaiser(b.Click)
mock.define Ordered {
    b.Click |> subscription expected once
}
mock.verify (fun()->
    b.Click |> Event.add (fun _ -> System.Console.WriteLine "clicked!" )
    clickRaiser.Raise(b, new EventArgs()) // this prints "clicked!"
)
```

Mock statements can be combined in no particular order

```fsharp
o.Call(1) |> returns 1 |> only_if_argument [Is.NotNull()] |> expected at_least_once
```
