FsMocks / FsharpMocks
=======
F# mock library
---------------------


FsMocks is an object mocking library written in F#. It is actually a wrapper around [Rhino.Mocks](http://ayende.com/wiki/Rhino+Mocks+Documentation.ashx) that simplifies mocking with F#. 
The API is simple and straightforward because it uses a human-friendly DSL syntax. 
It can be combined with other test frameworks (NUnit, xUnit, FsUnit, etc.)

Project status
---------------------
The project is not production-ready but usable. I am still making a few syntax changes.


NuGet
---------------------
Get the latest package [here](https://www.nuget.org/packages/FsMocks/)

Samples
---------------------
This sample creates a new IList and expects calls to the methods _Add()_ and _Clear()_, in any order, but with some constraints.

```fsharp
open System.Collections
open FsMocks.Syntax
// create mock object and repository
use mock = new FsMockRepository()	// use the keyword 'let' instead of 'use' if you don't want automatic verification (But why would you want that anyway?)
let mylist1:IList = mock.strict []

// define mock statements
mock.define Unordered {
  ~~ mylist1.Add "e" |> returns 2 |> expected once |> only_if_argument [Is.NotNull()]
  ~~ mylist1.Clear() |> expected twice
}

// run test.
mylist1.Clear()
mylist1.Add("another argument") |> should equal 2  // FsUnit syntax
mylist1.Clear()
// FsMocks will automatically verify expectations at the end of the test. The test will fail if any unexpected call was made
```

A mock definition can either be _Unordered_ or _Ordered_. 
It takes a series of mock statements.

A mock statement begins with `~~`, followed by the call or property to mock, followed by mock directives:


```fsharp
// the call is expected twice
~~ o.call() |> expected twice 

// the mock object will return 219 when this expectation is satisfied
~~ o.call("some arg") |> returns 219

// the call is expected only if it respects the given constraints
~~ o.call(arg) |> only_if_argument [Is.NotNull()] 

// Property1 will be implemented as a simple get/set property
~~ o.Property1 |> implement_as_property

// the most powerful statement : the call is manually implemented
let myCustomClearImplementation() = System.Console.WriteLine("list cleared!!!")
~~ list.Clear() |> implement_as (new Action(myCustomClearImplementation))

// throws an exception whenever a method is called
~~ o.call() |> throws (new Exception("Something went wrong!!"))

// subscribe to an event and simulate an event
let b:Button = mock.strict []
let clickRaiser = mock.getEventRaiser(b.Click)
mock.define Ordered {
    b.Click |> subscription expected once
}
b.Click |> Event.add (fun _ -> System.Console.WriteLine "clicked!" )
clickRaiser.Raise(b, new EventArgs()) // this prints "clicked!"
```

Mock directives can be combined in no particular order, except for the `returns` directive which must be on the first position.

```fsharp
~~ o.Call(1) |> returns 1 |> only_if_argument [Is.NotNull()] |> expected at_least_once
```

Mock definitions can be nested:
```fsharp
let [<Fact>] ``1b) 2 Ordered nested in 1 Unordered``() =
	// create mocks
	use mock = new FsMockRepository()
	let list:int IList = mock.strict []
	// mock definition
	mock.define Unordered {
		mock.define Ordered {
			~~ list.Add(1) |> expected once
			~~ list.Add(2) |> expected once
		}
		mock.define Ordered {
			~~ list.Add(3) |> expected twice
			~~ list.Contains(5) |> returns true |> expected once
		}
	}

	// run test
	list.Add(3)
	list.Add(3)
	Assert.True(list.Contains(5))
	list.Add(1)
	list.Add(2)
	// verify expectations
```
