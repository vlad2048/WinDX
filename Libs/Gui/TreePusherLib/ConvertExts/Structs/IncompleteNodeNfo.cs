namespace TreePusherLib.ConvertExts.Structs;

public sealed record IncompleteNodeNfo<T>(
    TNod<T> ParentNod,
    T ChildNode
);