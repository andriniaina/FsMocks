module MockDefinitionBuilder

// The builder class.
type MockDefinitionBuilder() =
    member x.Return(value) = value
    member x.Zero() = ()
