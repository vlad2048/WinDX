namespace TreePusherLib.ConvertExts.Structs;

public sealed record ReconstructedTree<T>(
    TNod<T> Root,
    IncompleteNodeNfo<T>[] IncompleteNodes
);